using grabs.Graphics;
using grabs.Graphics.D3D11;
using Silk.NET.SDL;
using Surface = grabs.Graphics.Surface;
using Texture = grabs.Graphics.Texture;

public static class Program
{
    public static unsafe void Main(string[] args)
    {
        using Sdl sdl = Sdl.GetApi();

        if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

        const int width = 1280;
        const int height = 720;

        Window* window = sdl.CreateWindow("Clear Color test", Sdl.WindowposCentered, Sdl.WindowposCentered, width,
            height, (uint) WindowFlags.Vulkan);

        if (window == null)
            throw new Exception($"Failed to create window: {sdl.GetErrorS()}");
        
        InstanceDescription desc = new InstanceDescription();

        using Instance instance = Instance.Create(in desc, null);

        Adapter[] adapters = instance.EnumerateAdapters();
        foreach (Adapter adapter in adapters)
            Console.WriteLine(adapter);

        Surface surface;

        switch (instance.Backend)
        {
            case Backend.Unknown:
            case Backend.Vulkan:
                throw new NotImplementedException();

            case Backend.D3D11:
            {
                SysWMInfo info = new SysWMInfo();
                sdl.GetWindowWMInfo(window, &info);
                
                surface = new D3D11Surface(info.Info.Win.Hwnd);
                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }

        using Device device = instance.CreateDevice(surface);

        SwapchainDescription swapchainDesc =
            new SwapchainDescription(new Size2D(width, height), Format.B8G8R8A8_UNorm, 2, PresentMode.Fifo);

        using Swapchain swapchain = device.CreateSwapchain(surface, in swapchainDesc);

        using CommandList cl = device.CreateCommandList();

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

            Texture texture = swapchain.GetNextTexture();
            
            cl.Begin();

            RenderPassDescription pass =
                new RenderPassDescription(new ColorAttachmentDescription(texture, new Color4(1.0f, 0.5f, 0.25f, 1.0f)));
            cl.BeginRenderPass(in pass);
            cl.EndRenderPass();
            
            cl.End();
            device.ExecuteCommandList(cl);
            
            swapchain.Present();
        }
        
        sdl.DestroyWindow(window);
        sdl.Quit();
    }
}