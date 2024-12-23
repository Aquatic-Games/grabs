using grabs.Graphics;
using grabs.Graphics.D3D11;
using Silk.NET.SDL;
using Buffer = grabs.Graphics.Buffer;
using Surface = grabs.Graphics.Surface;
using Texture = grabs.Graphics.Texture;

namespace Tests.Graphics.SelfContainedQuad;

public static class Program
{
    public static unsafe void Main(string[] args)
    {
        using Sdl sdl = Sdl.GetApi();

        if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

        const int width = 1280;
        const int height = 720;

        sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) GLprofile.Core);

        Window* window = sdl.CreateWindow("Test", Sdl.WindowposCentered, Sdl.WindowposCentered, width,
            height, (uint) WindowFlags.Opengl);

        if (window == null)
            throw new Exception($"Failed to create window: {sdl.GetErrorS()}");

        void* glContext = sdl.GLCreateContext(window);
        sdl.GLMakeCurrent(window, glContext);
        
        InstanceDescription desc = new InstanceDescription(true, Backend.LegacyGL);

        using Instance instance = Instance.Create(in desc, new WindowProvider(null, s => (nint) sdl.GLGetProcAddress(s)));

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
            new SwapchainDescription(width, height, Format.B8G8R8A8_UNorm, 2, PresentMode.Fifo);

        using Swapchain swapchain = device.CreateSwapchain(surface, in swapchainDesc);

        using CommandList cl = device.CreateCommandList();

        ReadOnlySpan<float> vertices =
        [
            -0.5f, +0.5f,    1.0f, 0.0f, 0.0f,
            +0.5f, +0.5f,    0.0f, 1.0f, 0.0f,
            +0.5f, -0.5f,    0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f,    0.0f, 0.0f, 0.0f
        ];

        ReadOnlySpan<ushort> indices = 
        [
            0, 1, 3, 
            1, 2, 3
        ];

        using Buffer vertexBuffer = device.CreateBuffer(BufferType.Vertex, in vertices);
        using Buffer indexBuffer = device.CreateBuffer(BufferType.Index, in indices);

        byte[] vertSpv = File.ReadAllBytes("Test_v.spv");
        byte[] pixlSpv = File.ReadAllBytes("Test_p.spv");

        using ShaderModule vertModule = device.CreateShaderModule(ShaderStage.Vertex, vertSpv, "VSMain");
        using ShaderModule pixlModule = device.CreateShaderModule(ShaderStage.Pixel, pixlSpv, "PSMain");

        ReadOnlySpan<InputLayoutDescription> inputLayout =
        [
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32B32_Float, 8, 0, InputType.PerVertex)
        ];

        PipelineDescription pipelineDesc = new PipelineDescription(vertModule, pixlModule, in inputLayout);

        using Pipeline pipeline = device.CreatePipeline(in pipelineDesc);

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

            Viewport vp = new Viewport(0, 0, width, height);
            cl.SetViewport(in vp);
            
            cl.SetPipeline(pipeline);
            cl.SetVertexBuffer(0, vertexBuffer, 5 * sizeof(float), 0);
            cl.SetIndexBuffer(indexBuffer, Format.R16_UInt);
            cl.DrawIndexed(6);
            
            cl.EndRenderPass();
            cl.End();
            
            device.ExecuteCommandList(cl);
            swapchain.Present();
        }
        
        sdl.DestroyWindow(window);
        sdl.Quit();
    }
}

class WindowProvider : IWindowProvider
{
    public readonly Func<string[]> GetVulkanInstanceExtensionsFunc;

    public readonly Func<string, nint> GetGLProcAddressFunc;

    public WindowProvider(Func<string[]> getVulkanInstanceExtensionsFunc, Func<string, IntPtr> getGlProcAddressFunc)
    {
        GetVulkanInstanceExtensionsFunc = getVulkanInstanceExtensionsFunc;
        GetGLProcAddressFunc = getGlProcAddressFunc;
    }

    public string[] GetVulkanInstanceExtensions()
    {
        throw new NotImplementedException();
    }

    public nint GetGLProcAddress(string name)
    {
        return GetGLProcAddressFunc(name);
    }
}