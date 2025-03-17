using System.Numerics;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.Exceptions;
using grabs.Graphics.Vulkan;
using grabs.ShaderCompiler;
using Silk.NET.SDL;
using StbImageSharp;
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

    Window* window = sdl.CreateWindow("grabs.Graphics.Tests", Sdl.WindowposCentered, Sdl.WindowposCentered, 1280, 720, (uint) WindowFlags.Resizable);

    if (window == null)
        throw new Exception($"Failed to create window: {sdl.GetErrorS()}");
    
    Instance.RegisterBackend<D3D11Backend>();
    Instance.RegisterBackend<VulkanBackend>();
    
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
        -0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
        -0.5f, +0.5f, 0.0f, 0.0f, 0.0f,
        +0.5f, +0.5f, 0.0f, 1.0f, 0.0f,
        +0.5f, -0.5f, 0.0f, 1.0f, 1.0f
    };

    ushort[] indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    Buffer vertexBuffer = device.CreateBuffer(BufferUsage.Vertex, vertices);
    Buffer indexBuffer = device.CreateBuffer(BufferUsage.Index, indices);
    //Buffer constantBuffer = device.CreateBuffer(BufferUsage.Constant, Matrix4x4.Identity);

    ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("/home/aqua/Pictures/BAGELMIP.png"),
        ColorComponents.RedGreenBlueAlpha);

    Texture texture = device.CreateTexture<byte>(
        TextureInfo.Texture2D(new Size2D((uint) result.Width, (uint) result.Height), Format.R8G8B8A8_UNorm,
            TextureUsage.Sampled), result.Data.AsSpan());

    DescriptorLayout layout = device.CreateDescriptorLayout(new DescriptorLayoutInfo()
    {
        Bindings =
        [
            new DescriptorBinding(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex),
            new DescriptorBinding(1, DescriptorType.Texture, ShaderStage.Pixel)
        ]
    });
    
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
        InputLayout =
        [
            new InputElement(Format.R32G32B32_Float, 0, 0),
            new InputElement(Format.R32G32_Float, 12, 0)
        ],
        Descriptors = [layout],
        Constants = [new ConstantLayout(ShaderStage.Vertex, 0, (uint) sizeof(Matrix4x4))]
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
        
        Texture swapchainTexture = swapchain.GetNextTexture();
        
        cl.Begin();
        
        h += 0.05f;
        //cl.UpdateBuffer(constantBuffer, Matrix4x4.CreateRotationZ(h));
        cl.PushConstant(pipeline, ShaderStage.Vertex, 0, Matrix4x4.CreateRotationZ(h));
        
        cl.BeginRenderPass(new RenderPassInfo(new ColorAttachmentInfo(swapchainTexture, new ColorF(1.0f, 0.5f, 0.25f))));
        
        cl.SetViewport(new Viewport(0, 0, swapchainTexture.Size.Width, swapchainTexture.Size.Height));

        cl.SetPipeline(pipeline);

        cl.PushDescriptors(0, pipeline,
        [
            //new Descriptor(0, DescriptorType.ConstantBuffer, buffer: constantBuffer),
            new Descriptor(1, DescriptorType.Texture, texture: texture)
        ]);
        
        cl.SetVertexBuffer(0, vertexBuffer, 5 * sizeof(float));
        cl.SetIndexBuffer(indexBuffer, Format.R16_UInt);
        cl.DrawIndexed(6);
        
        //cl.UpdateBuffer(constantBuffer, Matrix4x4.CreateTranslation(float.Sin(h), 0, 0));
        cl.PushConstant(pipeline, ShaderStage.Vertex, 0, Matrix4x4.CreateTranslation(float.Sin(h), 0, 0));
        
        cl.DrawIndexed(6);
        
        cl.EndRenderPass();
        
        cl.End();
        
        device.ExecuteCommandList(cl);
        
        swapchain.Present();
    }
    
    device.WaitForIdle();
    
    pipeline.Dispose();
    layout.Dispose();
    texture.Dispose();
    //constantBuffer.Dispose();
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