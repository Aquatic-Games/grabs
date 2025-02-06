using grabs;
using grabs.Core;
using Silk.NET.SDL;
using Surface = grabs.Surface;

GrabsLog.LogMessage += (severity, source, message, _, _) => Console.WriteLine($"{severity} - {source}: {message}");

unsafe
{
    Sdl sdl = Sdl.GetApi();

    if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
        throw new Exception($"Failed to initialize SDL. {sdl.GetErrorS()}");

    Window* window = sdl.CreateWindow("grabs.Tests", Sdl.WindowposCentered, Sdl.WindowposCentered, 1280, 720, 0);

    if (window == null)
        throw new Exception($"Failed to create window: {sdl.GetErrorS()}");
    
    InstanceInfo info = new InstanceInfo(Backend.Vulkan, "grabs.Tests", true);

    Instance instance = Instance.Create(info);

    Adapter[] adapters = instance.EnumerateAdapters();

    foreach (Adapter adapter in adapters)
        Console.WriteLine(adapter.ToString());

    SysWMInfo wmInfo = new SysWMInfo();
    sdl.GetVersion(&wmInfo.Version);
    if (!sdl.GetWindowWMInfo(window, &wmInfo))
        throw new Exception($"Failed to get WM info: {sdl.GetErrorS()}");

    SurfaceInfo surfaceInfo;
    
    switch (wmInfo.Subsystem)
    {
        case SysWMType.Windows:
            surfaceInfo = SurfaceInfo.Windows(wmInfo.Info.Win.HInstance, wmInfo.Info.Win.Hwnd);
            break;
        case SysWMType.Wayland:
            surfaceInfo = SurfaceInfo.Wayland((nint) wmInfo.Info.Wayland.Display, (nint) wmInfo.Info.Wayland.Surface);
            break;
        case SysWMType.X11:
            surfaceInfo = SurfaceInfo.Xlib((nint) wmInfo.Info.X11.Display, (nint) wmInfo.Info.X11.Window);
            break;
        
        default:
            throw new PlatformNotSupportedException();
    }

    Surface surface = instance.CreateSurface(in surfaceInfo);

    Device device = instance.CreateDevice(surface);

    Swapchain swapchain =
        device.CreateSwapchain(surface, new SwapchainInfo(1280, 720, Format.B8G8R8A8_UNorm, PresentMode.Fifo, 2));

    CommandList cl = device.CreateCommandList();
    
    bool alive = true;
    while (alive)
    {
        Event winEvent;
        while (sdl.PollEvent(&winEvent) != 0)
        {
            switch ((EventType) winEvent.Type)
            {
                case EventType.Windowevent:
                {
                    switch ((WindowEventID) winEvent.Window.Event)
                    {
                        case WindowEventID.Close:
                            alive = false;
                            break;
                    }
                    
                    break;
                }
            }
        }
        
        swapchain.GetNextTexture();
        
        /*cl.Begin();
        Console.WriteLine("Begin");
        cl.End();
        Console.WriteLine("end");
        
        device.ExecuteCommandList(cl);
        Console.WriteLine("execute");*/
        
        swapchain.Present();
    }
    
    cl.Dispose();
    swapchain.Dispose();
    device.Dispose();
    surface.Dispose();
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Quit();
    sdl.Dispose();
}