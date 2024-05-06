using System;
using System.IO;
using System.Numerics;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using Buffer = grabs.Graphics.Buffer;

namespace grabs.Tests.Tests;

public class BasicTest : TestBase
{
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Pipeline _pipeline;
    
    public BasicTest() : base("Basic Test") { }

    protected override void Initialize()
    {
        base.Initialize();

        ReadOnlySpan<float> vertices = stackalloc float[]
        {
            -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // Bottom left, red
            -0.5f, +0.5f, 0.0f, 1.0f, 0.0f, // Top left, green
            +0.5f, +0.5f, 0.0f, 0.0f, 1.0f, // Top right, blue
            +0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // Bottom right, black
        };

        ReadOnlySpan<ushort> indices = stackalloc ushort[]
        {
            0, 1, 3,
            1, 2, 3
        };

        _vertexBuffer = Device.CreateBuffer(BufferType.Vertex, vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.Index, indices);

        string hlsl = File.ReadAllText("Shaders/Basic.hlsl");
        byte[] vSpv = Compiler.CompileToSpirV(hlsl, "Vertex", ShaderStage.Vertex);
        byte[] pSpv = Compiler.CompileToSpirV(hlsl, "Pixel", ShaderStage.Pixel);

        using ShaderModule vModule = Device.CreateShaderModule(ShaderStage.Vertex, vSpv, "Vertex");
        using ShaderModule pModule = Device.CreateShaderModule(ShaderStage.Pixel, pSpv, "Pixel");

        _pipeline = Device.CreatePipeline(new PipelineDescription(vModule, pModule, new[]
        {
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32B32_Float, 8, 0, InputType.PerVertex) // Color
        }, DepthStencilDescription.Disabled));
    }

    protected override void Draw()
    {
        base.Draw();
        
        CommandList.Begin();

        CommandList.BeginRenderPass(new RenderPassDescription(Framebuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
        
        CommandList.SetViewport(new Viewport(0, 0, 1280, 720));
        
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
}