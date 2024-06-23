using System;
using System.Diagnostics;
using System.Drawing;
using grabs.Graphics;
using grabs.Windowing;
using grabs.Windowing.Events;

namespace Common;

public abstract class SampleBase : IDisposable
{
    private bool _alive;
    
    private Surface _surface;
    private Swapchain _swapchain;
    private Texture _swapchainTexture;
    
    public readonly Window Window;
    
    public readonly Instance Instance;
    public readonly Device Device;

    public readonly CommandList CommandList;

    public readonly Framebuffer SwapchainFramebuffer;

    public SampleBase(string title, Size? size = null)
    {
        Size winSize = size ?? new Size(800, 600);
        
        WindowInfo winInfo = new WindowInfo(title, winSize);
        Window = new Window(winInfo);

        Instance = Window.CreateInstance();
        _surface = Window.CreateSurface();

        Device = Instance.CreateDevice(_surface);
        CommandList = Device.CreateCommandList();

        _swapchain = Device.CreateSwapchain(new SwapchainDescription((uint) winSize.Width, (uint) winSize.Height,
            presentMode: PresentMode.VerticalSync));

        _swapchainTexture = _swapchain.GetSwapchainTexture();
        SwapchainFramebuffer = Device.CreateFramebuffer(_swapchainTexture);
    }

    protected virtual void Initialize() { }
    protected virtual void Update(float dt) { }
    protected virtual void Draw() { }

    public void Run()
    {
        _alive = true;
        
        Initialize();
        
        Stopwatch sw = Stopwatch.StartNew();
        
        while (_alive)
        {
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        _alive = false;
                        break;
                }
            }

            float dt = (float) sw.Elapsed.TotalSeconds;
            sw.Restart();
            
            Update(dt);
            Draw();
            
            _swapchain.Present();
        }
    }
    
    public virtual void Dispose()
    {
        SwapchainFramebuffer.Dispose();
        _swapchainTexture.Dispose();
        _swapchain.Dispose();
        CommandList.Dispose();
        Device.Dispose();
        _surface.Dispose();
        Instance.Dispose();
        Window.Dispose();
    }
}