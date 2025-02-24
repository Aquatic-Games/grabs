using System.Numerics;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.Exceptions;
using grabs.ShaderCompiler;
using Silk.NET.SDL;
using Buffer = grabs.Graphics.Buffer;
using Surface = grabs.Graphics.Surface;
using Texture = grabs.Graphics.Texture;

//Console.WriteLine(Instance.IsBackendSupported(Backend.Vulkan));

GrabsLog.LogMessage += (severity, source, message, _, _) => Console.WriteLine($"{severity} - {source}: {message}");

unsafe
{
    Sdl sdl = Sdl.GetApi();

    if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
        throw new Exception($"Failed to initialize SDL. {sdl.GetErrorS()}");

    Window* window = sdl.CreateWindow("grabs.Graphics.Tests", Sdl.WindowposCentered, Sdl.WindowposCentered, 1280, 720, 0);

    if (window == null)
        throw new Exception($"Failed to create window: {sdl.GetErrorS()}");
    
    InstanceInfo info = new InstanceInfo("grabs.Graphics.Tests", debug: true);
    
    Instance instance;

    try
    {
        instance = Instance.Create(info);
    }
    catch (DebugLayersNotFoundException e)
    {
        sdl.ShowSimpleMessageBox((uint) MessageBoxFlags.Error, "Error", e.Message, null);
        throw;
    }

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
    
    Console.WriteLine(string.Join(", ", surface.EnumerateSupportedFormats(device.Adapter)));
    Format swapchainFormat = surface.GetOptimalSwapchainFormat(device.Adapter);
    Console.WriteLine(swapchainFormat);

    Swapchain swapchain =
        device.CreateSwapchain(new SwapchainInfo(surface, new Size2D(1280, 720), swapchainFormat, PresentMode.Fifo, 2));

    CommandList cl = device.CreateCommandList();

    float[] vertices =
    {
        -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
        -0.5f, +0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
        +0.5f, +0.5f, 0.0f, 0.0f, 0.0f, 1.0f,
        +0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f
    };

    ushort[] indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    Buffer vertexBuffer = device.CreateBuffer(BufferType.Vertex, vertices, true);
    Buffer indexBuffer = device.CreateBuffer(BufferType.Index, indices);
    Buffer constantBuffer = device.CreateBuffer(BufferType.Constant, Matrix4x4.CreateTranslation(0.5f, 0, 0));

    string hlsl = File.ReadAllText("Shader.hlsl");

    ShaderModule vertexModule =
        device.CreateShaderModule(ShaderStage.Vertex, Compiler.CompileHlsl(ShaderStage.Vertex, hlsl, "VSMain"), "VSMain");
    
    ShaderModule pixelModule =
        device.CreateShaderModule(ShaderStage.Pixel, Compiler.CompileHlsl(ShaderStage.Pixel, hlsl, "PSMain"), "PSMain");

    PipelineInfo pipelineInfo = new PipelineInfo()
    {
        VertexShader = vertexModule,
        PixelShader = pixelModule,
        ColorAttachmentFormats = [swapchain.SwapchainFormat],
        VertexBuffers = [new VertexBufferInfo(0, 6 * sizeof(float))],
        InputLayout =
        [
            new InputLayoutInfo(Format.R32G32B32_Float, 0, 0),
            new InputLayoutInfo(Format.R32G32B32_Float, 12, 0)
        ]
    };
    
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
        
        cl.BeginRenderPass(new RenderPassInfo(new ColorAttachmentInfo(texture, new ColorF(1.0f, 0.5f, 0.25f))));
        
        cl.SetViewport(new Viewport(0, 0, 1280, 720));

        cl.SetPipeline(pipeline);
        cl.SetVertexBuffer(0, vertexBuffer);
        cl.SetIndexBuffer(indexBuffer, Format.R16_UInt);
        cl.DrawIndexed(6);
        
        cl.EndRenderPass();
        
        cl.End();
        
        device.ExecuteCommandList(cl);
        
        swapchain.Present();
    }
    
    device.WaitForIdle();
    
    pipeline.Dispose();
    constantBuffer.Dispose();
    indexBuffer.Dispose();
    vertexBuffer.Dispose();
    cl.Dispose();
    swapchain.Dispose();
    device.Dispose();
    surface.Dispose();
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Quit();
    sdl.Dispose();
}