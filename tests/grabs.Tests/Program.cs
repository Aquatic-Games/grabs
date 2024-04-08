using System.Numerics;
using grabs;
using grabs.D3D11;
using grabs.Vulkan;
using Silk.NET.SDL;
using Buffer = grabs.Buffer;
using Surface = grabs.Surface;

Sdl sdl = Sdl.GetApi();

unsafe
{
    if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
        throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

    const uint width = 1280;
    const uint height = 720;

    Window* window = sdl.CreateWindow("Test", Sdl.WindowposCentered, Sdl.WindowposCentered, (int) width, (int) height,
        (uint) WindowFlags.Vulkan);

    SysWMInfo info = new SysWMInfo();
    sdl.GetWindowWMInfo(window, &info);

    if (window == null)
        throw new Exception($"Failed to create SDL window: {sdl.GetErrorS()}");
    
    //Instance instance = new D3D11Instance();

    uint instanceCount = 0;
    sdl.VulkanGetInstanceExtensions(window, ref instanceCount, (byte**) null);
    string[] extensions = new string[instanceCount];
    sdl.VulkanGetInstanceExtensions(window, ref instanceCount, extensions);
    
    Instance instance = new VkInstance(extensions);

    /*Adapter[] adapters = instance.EnumerateAdapters();
    Console.WriteLine(string.Join('\n', adapters));

    Device device = instance.CreateDevice();

    Surface surface = new D3D11Surface(info.Info.Win.Hwnd);
    Swapchain swapchain = device.CreateSwapchain(surface, new SwapchainDescription(width, height, Format.B8G8R8A8_UNorm, 2, PresentMode.VerticalSync));
    ColorTarget swapchainTarget = swapchain.GetColorTarget();

    CommandList commandList = device.CreateCommandList();

    ReadOnlySpan<float> vertices = stackalloc float[]
    {
        -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,
        -0.5f, +0.5f, 0.0f, 1.0f, 0.0f,
        +0.5f, +0.5f, 0.0f, 0.0f, 1.0f,
        +0.5f, -0.5f, 1.0f, 1.0f, 1.0f
    };

    ReadOnlySpan<uint> indices = stackalloc uint[]
    {
        0, 1, 3,
        1, 2, 3
    };

    Buffer vertexBuffer =
        device.CreateBuffer(new BufferDescription(BufferType.Vertex, (uint) (vertices.Length * sizeof(float))), vertices);
    Buffer indexBuffer =
        device.CreateBuffer(new BufferDescription(BufferType.Index, (uint) (indices.Length * sizeof(uint))), indices);
        
    */
    
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
        
        /*commandList.Begin();

        commandList.BeginRenderPass(new RenderPassDescription(new ReadOnlySpan<ColorTarget>(ref swapchainTarget),
            new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
        commandList.EndRenderPass();
        
        commandList.End();
        
        device.ExecuteCommandList(commandList);
        swapchain.Present();*/
    }
    
    /*indexBuffer.Dispose();
    vertexBuffer.Dispose();
    
    commandList.Dispose();
    swapchain.Dispose();
    surface.Dispose();
    device.Dispose();*/
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Dispose();
}
