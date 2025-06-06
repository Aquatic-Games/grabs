﻿using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.Vulkan;
using grabs.ShaderCompiler;
using Silk.NET.SDL;
using Buffer = grabs.Graphics.Buffer;
using Surface = grabs.Graphics.Surface;
using Texture = grabs.Graphics.Texture;

GrabsLog.LogMessage += (severity, message, line, file) => Console.WriteLine($"{severity}: {message}");
Instance.RegisterBackend<D3D11Backend>();
Instance.RegisterBackend<VulkanBackend>();

const string ShaderCode = @"
struct VSInput
{
    float2 Position: POSITION0;
    float3 Color:    COLOR0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float3 Color: COLOR0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = float4(input.Position, 0.0, 1.0);
    output.Color = input.Color;

    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = float4(input.Color, 1.0);

    return output;
}
";

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

    ReadOnlySpan<float> vertices =
    [
        -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,
        -0.5f, +0.5f, 0.0f, 1.0f, 0.0f,
        +0.5f, +0.5f, 0.0f, 0.0f, 1.0f,
        +0.5f, -0.5f, 0.0f, 0.0f, 0.0f
    ];

    ReadOnlySpan<short> indices =
    [
        0, 1, 3,
        1, 2, 3
    ];

    Buffer vertexBuffer = device.CreateBuffer(BufferUsage.Vertex, vertices);
    Buffer indexBuffer = device.CreateBuffer(BufferUsage.Index, indices);

    ShaderModule vertexModule = device.CreateShaderModuleFromHlsl(ShaderStage.Vertex, ShaderCode, "VSMain");
    ShaderModule pixelModule = device.CreateShaderModuleFromHlsl(ShaderStage.Pixel, ShaderCode, "PSMain");

    GraphicsPipelineInfo pipelineInfo = new()
    {
        VertexShader = vertexModule,
        PixelShader = pixelModule,
        ColorAttachments = [new ColorAttachmentDescription(swapchain.BufferFormat)],
        InputLayout =
        [
            new InputElementDescription(SemanticType.Position, 0, Format.R32G32_Float, 0),
            new InputElementDescription(SemanticType.Color, 0, Format.R32G32B32_Float, 8)
        ]
    };

    Pipeline pipeline = device.CreateGraphicsPipeline(in pipelineInfo);
    
    pixelModule.Dispose();
    vertexModule.Dispose();

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
        
        cl.SetGraphicsPipeline(pipeline);
        cl.SetVertexBuffer(0, vertexBuffer, 5 * sizeof(float));
        cl.SetIndexBuffer(indexBuffer, Format.R16_UInt);
        
        cl.DrawIndexed(6);
        
        cl.EndRenderPass();
        
        cl.End();
        
        device.ExecuteCommandList(cl);
        
        swapchain.Present();
    }
    
    pipeline.Dispose();
    indexBuffer.Dispose();
    vertexBuffer.Dispose();
    swapchain.Dispose();
    cl.Dispose();
    device.Dispose();
    surface.Dispose();
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Quit();
    sdl.Dispose();
}