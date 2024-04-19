using System.Numerics;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.GL43;
using grabs.ShaderCompiler.DXC;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using Buffer = grabs.Graphics.Buffer;
using Framebuffer = grabs.Graphics.Framebuffer;
using Surface = grabs.Graphics.Surface;
using Texture = grabs.Graphics.Texture;

const string shaderCode = """
                          struct VSInput
                          {
                              float2 Position: POSITION0;
                              float3 Color:    COLOR0;
                          };

                          struct VSOutput
                          {
                              float4 Position: SV_Position;
                              float4 Color:    COLOR0;
                          };

                          struct PSOutput
                          {
                              float4 Color: SV_Target0;
                          };

                          cbuffer TransformMatrix : register(b0)
                          {
                              float4x4 Transform;
                          }
                          
                          VSOutput Vertex(const in VSInput input)
                          {
                              VSOutput output;
                              
                              output.Position = mul(Transform, float4(input.Position, 0.0, 1.0));
                              output.Color = float4(input.Color, 1.0);
                              
                              return output;
                          }

                          PSOutput Pixel(const in VSOutput input)
                          {
                              PSOutput output;
                              
                              output.Color = input.Color;
                              
                              return output;
                          }

                          """;

Sdl sdl = Sdl.GetApi();

unsafe
{
    if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
        throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

    const uint width = 1280;
    const uint height = 720;

    const GraphicsApi api = GraphicsApi.D3D11;

    WindowFlags flags = WindowFlags.Shown;
    
    switch (api)
    {
        case GraphicsApi.OpenGL:
            sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) ContextProfileMask.CoreProfileBit);
            sdl.GLSetAttribute(GLattr.ContextMajorVersion, 4);
            sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);

            flags |= WindowFlags.Opengl;
            break;
        case GraphicsApi.D3D11:
            break;
        case GraphicsApi.OpenGLES:
            break;
        case GraphicsApi.Vulkan:
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }

    Window* window = sdl.CreateWindow("Test", Sdl.WindowposCentered, Sdl.WindowposCentered, (int) width, (int) height,
        (uint) flags);
    
    if (window == null)
        throw new Exception($"Failed to create SDL window: {sdl.GetErrorS()}");

    void* glCtx;
    if (api is GraphicsApi.OpenGL or GraphicsApi.OpenGLES)
    {
        glCtx = sdl.GLCreateContext(window);
        sdl.GLMakeCurrent(window, glCtx);
    }

    SysWMInfo info = new SysWMInfo();
    sdl.GetWindowWMInfo(window, &info);

    Instance instance;
    Surface surface;
    switch (api)
    {
        case GraphicsApi.D3D11:
            instance = new D3D11Instance();
            surface = new D3D11Surface(info.Info.Win.Hwnd);
            break;
        case GraphicsApi.OpenGL:
            instance = new GL43Instance(s => (nint) sdl.GLGetProcAddress(s));
            surface = new GL43Surface(i => { sdl.GLSetSwapInterval(i); sdl.GLSwapWindow(window); });
            break;
        case GraphicsApi.OpenGLES:
            break;
        case GraphicsApi.Vulkan:
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
    
    Console.WriteLine(instance.Api);

    Adapter[] adapters = instance.EnumerateAdapters();
    Console.WriteLine(string.Join('\n', adapters));

    Device device = instance.CreateDevice();
    
    Swapchain swapchain = device.CreateSwapchain(surface, new SwapchainDescription(width, height, Format.B8G8R8A8_UNorm, 2, PresentMode.VerticalSync));
    Texture swapchainTexture = swapchain.GetSwapchainTexture();
    Framebuffer swapchainBuffer = device.CreateFramebuffer(new ReadOnlySpan<Texture>(ref swapchainTexture));

    CommandList commandList = device.CreateCommandList();

    ReadOnlySpan<float> vertices = stackalloc float[]
    {
        -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,
        -0.5f, +0.5f, 0.0f, 1.0f, 0.0f,
        +0.5f, +0.5f, 0.0f, 0.0f, 1.0f,
        +0.5f, -0.5f, 0.0f, 0.0f, 0.0f
    };

    ReadOnlySpan<uint> indices = stackalloc uint[]
    {
        0, 1, 3,
        1, 2, 3
    };

    Buffer vertexBuffer = device.CreateBuffer(BufferType.Vertex, vertices);
    Buffer indexBuffer = device.CreateBuffer(BufferType.Index, indices);

    Buffer transformBuffer = device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);

    ShaderModule vertexModule = device.CreateShaderModule(ShaderStage.Vertex,
        Compiler.CompileToSpirV(shaderCode, "Vertex", ShaderStage.Vertex), "Vertex");
    ShaderModule pixelModule = device.CreateShaderModule(ShaderStage.Pixel,
        Compiler.CompileToSpirV(shaderCode, "Pixel", ShaderStage.Pixel), "Pixel");

    Pipeline pipeline = device.CreatePipeline(new PipelineDescription(vertexModule, pixelModule,
        new[]
        {
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32B32_Float, 8, 0, InputType.PerVertex)
        }));
    
    pixelModule.Dispose();
    vertexModule.Dispose();

    float rotation = 0;
    
    /*commandList.Begin();
    
    commandList.BeginRenderPass(new RenderPassDescription(swapchainBuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f), LoadOp.Clear));
    commandList.EndRenderPass();
    
    commandList.End();*/
    
    device.ExecuteCommandList(commandList);
    
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
        
        commandList.Begin();
        
        commandList.UpdateBuffer(transformBuffer, 0, Matrix4x4.CreateRotationZ(rotation));
        rotation += 0.01f;
        
        commandList.SetViewport(new Viewport(100, 100, width / 2, height / 2));

        commandList.BeginRenderPass(new RenderPassDescription(swapchainBuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
        
        commandList.SetPipeline(pipeline);
        commandList.SetVertexBuffer(0, vertexBuffer, 5 * sizeof(float), 0);
        commandList.SetIndexBuffer(indexBuffer, Format.R32_UInt);
        
        commandList.SetConstantBuffer(0, transformBuffer);
        
        commandList.DrawIndexed(6);
        
        commandList.EndRenderPass();
        
        commandList.End();
        
        device.ExecuteCommandList(commandList);
        swapchain.Present();
    }
    
    pipeline.Dispose();
    
    indexBuffer.Dispose();
    vertexBuffer.Dispose();
    
    commandList.Dispose();
    swapchain.Dispose();
    surface.Dispose();
    device.Dispose();
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Dispose();
}
