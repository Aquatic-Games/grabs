using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using Common;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using Buffer = grabs.Graphics.Buffer;

namespace Tutorial;

public class Main : SampleBase
{
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Pipeline _pipeline;
    
    protected override void Initialize()
    {
        ReadOnlySpan<float> quadVertices = stackalloc float[]
        {
            -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,
            +0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
            +0.5f, +0.5f, 0.0f, 0.0f, 1.0f,
            -0.5f, +0.5f, 0.0f, 0.0f, 0.0f,
        };

        ReadOnlySpan<ushort> quadIndices = stackalloc ushort[]
        {
            0, 1, 3,
            1, 2, 3
        };

        _vertexBuffer = Device.CreateBuffer(BufferType.Vertex, quadVertices);
        _indexBuffer = Device.CreateBuffer(BufferType.Index, quadIndices);

        string shader = File.ReadAllText("Shader.hlsl");

        byte[] vertexSpirv = Compiler.CompileToSpirV(shader, "VSMain", ShaderStage.Vertex);
        byte[] pixelSpirv = Compiler.CompileToSpirV(shader, "PSMain", ShaderStage.Pixel);

        using ShaderModule vertexModule = Device.CreateShaderModule(ShaderStage.Vertex, vertexSpirv, "VSMain");
        using ShaderModule pixelModule = Device.CreateShaderModule(ShaderStage.Pixel, pixelSpirv, "PSMain");

        InputLayoutDescription[] inputLayout =
        [
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32B32_Float, 8, 0, InputType.PerVertex) // Color
        ];

        PipelineDescription pipelineDesc = new PipelineDescription(vertexModule, pixelModule, inputLayout,
            DepthStencilDescription.Disabled, RasterizerDescription.CullNone, null);

        _pipeline = Device.CreatePipeline(pipelineDesc);
    }

    protected override void Draw()
    {
        CommandList.Begin();
        
        Size windowSize = Window.FramebufferSize;
        CommandList.SetViewport(new Viewport(0, 0, (uint) windowSize.Width, (uint) windowSize.Height));
        
        CommandList.BeginRenderPass(new RenderPassDescription(SwapchainFramebuffer, new Vector4(0.2f, 0.3f, 0.3f, 1.0f)));
        
        CommandList.SetPipeline(_pipeline);
        CommandList.SetVertexBuffer(0, _vertexBuffer, 5 * sizeof(float), 0);
        CommandList.SetIndexBuffer(_indexBuffer, Format.R16_UInt);
        
        CommandList.DrawIndexed(6);
        
        CommandList.EndRenderPass();
        CommandList.End();
        
        Device.ExecuteCommandList(CommandList);
    }

    public override void Dispose()
    {
        _pipeline.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
        
        base.Dispose();
    }

    public Main() : base("1.2 - Simple Quad") { }
}