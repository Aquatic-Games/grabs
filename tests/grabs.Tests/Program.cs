using grabs;
using grabs.Core;
using grabs.ShaderCompiler;
using Silk.NET.SDL;
using Surface = grabs.Surface;
using Texture = grabs.Texture;

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
        device.CreateSwapchain(surface, new SwapchainInfo(new Size2D(1280, 720), Format.B8G8R8A8_UNorm, PresentMode.Fifo, 2));

    CommandList cl = device.CreateCommandList();

    string hlsl = File.ReadAllText("Shader.hlsl");

    ShaderModule vertexModule =
        device.CreateShaderModule(ShaderStage.Vertex, Compiler.CompileHlsl(ShaderStage.Vertex, hlsl, "VSMain"), "VSMain");
    
    ShaderModule pixelModule =
        device.CreateShaderModule(ShaderStage.Pixel, Compiler.CompileHlsl(ShaderStage.Pixel, hlsl, "PSMain"), "PSMain");

    PipelineInfo pipelineInfo = new PipelineInfo(vertexModule, pixelModule, Format.B8G8R8A8_UNorm);

    Pipeline pipeline = device.CreatePipeline(in pipelineInfo);
    
    pixelModule.Dispose();
    vertexModule.Dispose();

    float h = 0;
    
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

        float x = 1 - float.Abs((h / 60.0f) % 2.0f - 1);

        (float r, float g, float b) = h switch
        {
            >= 0 and < 60 => (1, x, 0),
            >= 60 and < 120 => (x, 1, 0),
            >= 120 and < 180 => (0, 1, x),
            >= 180 and < 240 => (0, x, 1),
            >= 240 and < 300 => (x, 0, 1),
            >= 300 and < 360 => (1, 0, x),
            _ => (0.0f, 0.0f, 0.0f)
        };
        
        //Console.WriteLine($"h: {h}, r: {r}, g: {g}, b: {b}");

        h += 1;
        if (h >= 360)
            h = 0;
        
        cl.BeginRenderPass(new RenderPassInfo(new ColorAttachmentInfo(texture, new ColorF(r, g, b))));
        
        cl.SetViewport(new Viewport(0, 0, 1280, 720));

        cl.SetPipeline(pipeline);
        cl.Draw(3);
        
        cl.EndRenderPass();
        
        cl.End();
        
        device.ExecuteCommandList(cl);
        
        swapchain.Present();
    }
    
    device.WaitForIdle();
    
    pipeline.Dispose();
    cl.Dispose();
    swapchain.Dispose();
    device.Dispose();
    surface.Dispose();
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Quit();
    sdl.Dispose();
}