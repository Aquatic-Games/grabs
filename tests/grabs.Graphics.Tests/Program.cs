using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.Vulkan;
using Silk.NET.SDL;
using Surface = grabs.Graphics.Surface;
using Texture = grabs.Graphics.Texture;

GrabsLog.LogMessage += (severity, message, line, file) => Console.WriteLine($"{severity}: {message}");
Instance.RegisterBackend<VulkanBackend>();

unsafe
{
    Sdl sdl = Sdl.GetApi();

    if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
        throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

    Window* window =
        sdl.CreateWindow("grabs.Graphics.Tests", Sdl.WindowposCentered, Sdl.WindowposCentered, 1280, 720, 0);

    if (window == null)
        throw new Exception($"Failed to create window: {sdl.GetErrorS()}");
    
    Instance instance = Instance.Create(new InstanceInfo("test", true));

    Adapter[] adapters = instance.EnumerateAdapters();

    foreach (Adapter adapter in adapters)
        Console.WriteLine(adapter);

    SysWMInfo wmInfo = new SysWMInfo();
    sdl.GetVersion(&wmInfo.Version);
    sdl.GetWindowWMInfo(window, &wmInfo);

    SurfaceInfo surfaceInfo = wmInfo.Subsystem switch
    {
        SysWMType.Windows => SurfaceInfo.Windows(wmInfo.Info.Win.HInstance, wmInfo.Info.Win.Hwnd),
        SysWMType.X11 => SurfaceInfo.Xlib((nint) wmInfo.Info.X11.Display, (nint) wmInfo.Info.X11.Window),
        SysWMType.Wayland => SurfaceInfo.Wayland((nint) wmInfo.Info.Wayland.Display, (nint) wmInfo.Info.Wayland.Surface),
        _ => throw new NotSupportedException()
    };

    Surface surface = instance.CreateSurface(in surfaceInfo);
    Device device = instance.CreateDevice(surface);

    CommandList cl = device.CreateCommandList();
    
    Swapchain swapchain =
        device.CreateSwapchain(new SwapchainInfo(surface, new Size2D(1280, 720), Format.B8G8R8A8_UNorm,
            PresentMode.Fifo, 2));

    bool alive = true;
    while (alive)
    {
        Event sdlEvent;
        while (sdl.PollEvent(&sdlEvent) != 0)
        {
            switch ((EventType) sdlEvent.Type)
            {
                case EventType.Windowevent:
                {
                    switch ((WindowEventID) sdlEvent.Window.Event)
                    {
                        case WindowEventID.Close:
                            alive = false;
                            break;
                    }

                    break;
                }
            }
        }

        Texture texture = swapchain.GetNextTexture();
        
        cl.Begin();
        
        cl.BeginRenderPass(new ColorAttachmentInfo(texture, new ColorF(1.0f, 0.5f, 0.25f)));
        cl.EndRenderPass();
        
        cl.End();
        
        device.ExecuteCommandList(cl);
        
        swapchain.Present();
    }
    
    swapchain.Dispose();
    cl.Dispose();
    device.Dispose();
    surface.Dispose();
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Quit();
    sdl.Dispose();
}