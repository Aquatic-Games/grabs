using grabs;
using grabs.D3D11;
using Silk.NET.SDL;

Sdl sdl = Sdl.GetApi();

unsafe
{
    if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
        throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

    const uint width = 1280;
    const uint height = 720;

    Window* window = sdl.CreateWindow("Test", Sdl.WindowposCentered, Sdl.WindowposCentered, (int) width, (int) height,
        (uint) WindowFlags.Shown);

    SysWMInfo info = new SysWMInfo();
    sdl.GetWindowWMInfo(window, &info);

    if (window == null)
        throw new Exception($"Failed to create SDL window: {sdl.GetErrorS()}");
    
    Instance instance = new D3D11Instance();

    Adapter[] adapters = instance.EnumerateAdapters();
    Console.WriteLine(string.Join('\n', adapters));

    Device device = instance.CreateDevice();
    Swapchain swapchain =
        device.CreateSwapchain(new D3D11Surface(info.Info.Win.Hwnd), new SwapchainDescription(width, height, 2));

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
        
        swapchain.Present();
    }
    
    device.Dispose();
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Dispose();
}
