using System;
using System.Drawing;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.GL43;
using Silk.NET.SDL;
using Surface = grabs.Graphics.Surface;
using Texture = grabs.Graphics.Texture;

namespace grabs.Tests;

public unsafe abstract class TestBase : IDisposable
{
    private string _title;
    private Sdl _sdl;
    private Window* _window;
    private void* _glContext;

    public Instance Instance;
    public Device Device;

    public Surface Surface;
    
    public Swapchain Swapchain;
    public Texture ColorTexture;
    public Texture DepthTexture;
    public Framebuffer Framebuffer;

    public CommandList CommandList;

    protected TestBase(string title)
    {
        _title = title;
        _sdl = Sdl.GetApi();
    }

    protected virtual void Initialize() { }

    protected virtual void Update(float dt) { }

    protected virtual void Draw() { }

    protected virtual void Unload() { }

    public void Run(GraphicsApi api, Size size)
    {
        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception("Failed to initialize SDL.");

        WindowFlags flags = WindowFlags.Shown;
        
        switch (api)
        {
            case GraphicsApi.D3D11:
                break;
            
            case GraphicsApi.OpenGL:
                flags |= WindowFlags.Opengl;
                
                _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 4);
                _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);
                _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) GLprofile.Core);
                _sdl.GLSetAttribute(GLattr.DepthSize, 0);
                _sdl.GLSetAttribute(GLattr.StencilSize, 0);
                break;
            
            case GraphicsApi.OpenGLES:
            case GraphicsApi.Vulkan:
                throw new NotImplementedException();
            
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }

        _window = _sdl.CreateWindow(_title, Sdl.WindowposCentered, Sdl.WindowposCentered, size.Width, size.Height,
            (uint) flags);

        if (_window == null)
            throw new Exception("Failed to create SDL window.");

        switch (api)
        {
            case GraphicsApi.D3D11:
                SysWMInfo wmInfo = new SysWMInfo();
                _sdl.GetWindowWMInfo(_window, &wmInfo);
                
                Instance = new D3D11Instance();
                Surface = new D3D11Surface(wmInfo.Info.Win.Hwnd);
                break;
            case GraphicsApi.OpenGL:
                _glContext = _sdl.GLCreateContext(_window);
                _sdl.GLMakeCurrent(_window, _glContext);

                Instance = new GL43Instance(s => (nint) _sdl.GLGetProcAddress(s));
                Surface = new GL43Surface(i =>
                {
                    _sdl.GLSetSwapInterval(i);
                    _sdl.GLSwapWindow(_window);
                });
                break;
            
            case GraphicsApi.OpenGLES:
            case GraphicsApi.Vulkan:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }
        
        _sdl.SetWindowTitle(_window, _title + " - " + Instance.Api);

        Adapter[] adapters = Instance.EnumerateAdapters();
        Console.WriteLine($"Adapters:\n{string.Join('\n', adapters)}");

        Device = Instance.CreateDevice();
        Swapchain = Device.CreateSwapchain(Surface,
            new SwapchainDescription((uint) size.Width, (uint) size.Height, presentMode: PresentMode.VerticalSync));
        ColorTexture = Swapchain.GetSwapchainTexture();
        DepthTexture = Device.CreateTexture(new TextureDescription(TextureType.Texture2D, (uint) size.Width,
            (uint) size.Height, 1, Format.D32_Float, TextureUsage.None));

        Framebuffer = Device.CreateFramebuffer(new ReadOnlySpan<Texture>(ref ColorTexture), DepthTexture);

        CommandList = Device.CreateCommandList();
        
        Initialize();

        bool isOpen = true;
        while (isOpen)
        {
            Event winEvent;
            while (_sdl.PollEvent(&winEvent) != 0)
            {
                switch ((EventType) winEvent.Type)
                {
                    case EventType.Windowevent:
                        switch ((WindowEventID) winEvent.Window.Event)
                        {
                            case WindowEventID.Close:
                                isOpen = false;
                                break;
                        }

                        break;
                }
                
                Update(1 / 60f);
                Draw();
                
                Swapchain.Present();
            }
        }
    }
    
    public virtual void Dispose()
    {
        Framebuffer.Dispose();
        DepthTexture.Dispose();
        ColorTexture.Dispose();
        Swapchain.Dispose();
        Surface.Dispose();
        
        Device.Dispose();
        Instance.Dispose();
        
        if (_glContext != null)
            _sdl.GLDeleteContext(_glContext);
        
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
}