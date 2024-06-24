using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using Common;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using Buffer = grabs.Graphics.Buffer;

/*
 * Simple Quad tutorial.
 * This tutorial draws a colored quad to the screen.
 * In this tutorial, you will learn:
 *   - How a quad is represented by the GPU.
 *   - How coordinates are represented on the GPU.
 *   - How to create buffers, shader modules, and pipelines, plus what all of them are and are used for.
 *   - What a viewport is and how to set it.
 */

namespace Tutorial;

public class Main : SampleBase
{
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Pipeline _pipeline;
    
    protected override void Initialize()
    {
        // 0 -------- 1   A quad is made up of 2 right-angled triangles.
        // |        / |   Each number in the corner represents an "index", which you'll see in quadIndices down below.
        // |      /   |         +1 Y
        // |    /     |           |          GRABS uses the coordinate system to the left.
        // |  /       |    -1 X --+-- +1 X   Each index in the quadIndex array matches one of the vertices in quadVertices.
        // 3 -------- 2           |          If you match them up, you'll see how this quad is made.
        //                      -1 Y
        
        float[] quadVertices = new float[]
        {
            // Position     Color
            -0.5f, +0.5f,   1.0f, 0.0f, 0.0f,
            +0.5f, +0.5f,   0.0f, 1.0f, 0.0f,
            +0.5f, -0.5f,   0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f,   0.0f, 0.0f, 0.0f,
        };

        ushort[] quadIndices = new ushort[]
        {
            0, 1, 3,
            1, 2, 3
        };

        // Create our buffers from the data above.
        // The vertex buffer contains the vertices, and the index buffer tells the GPU which vertex to use for each
        // **real** vertex, which is drawn to the screen. This reduces the amount of data duplication.
        // At a small level like this, it doesn't matter. But once you get large models, it can make the difference.
        _vertexBuffer = Device.CreateBuffer(BufferType.Vertex, quadVertices);
        _indexBuffer = Device.CreateBuffer(BufferType.Index, quadIndices);

        string shader = File.ReadAllText("Shader.hlsl");

        // GRABS only accepts SPIR-V shaders. As such, we must first compile our HLSL shader to SPIR-V, using GRABS'
        // built-in optional shader compiler.
        byte[] vertexSpirv = Compiler.CompileToSpirV(shader, "VSMain", ShaderStage.Vertex);
        byte[] pixelSpirv = Compiler.CompileToSpirV(shader, "PSMain", ShaderStage.Pixel);

        // Create shader modules from our SPIR-V shaders.
        // Shader modules take the SPIR-V bytecode, and compile it into the GPU-specific machine code.
        // They are used in pipelines, and can be stored and reused.
        // Once used in a pipeline, we don't need to keep them around, so here, we just dispose them when done.
        using ShaderModule vertexModule = Device.CreateShaderModule(ShaderStage.Vertex, vertexSpirv, "VSMain");
        using ShaderModule pixelModule = Device.CreateShaderModule(ShaderStage.Pixel, pixelSpirv, "PSMain");

        // The input layout defines how the vertex data is passed to our shader.
        // This tells the GPU how to interpret the data inside our vertex buffer.
        InputLayoutDescription[] inputLayout =
        [
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32B32_Float, 8, 0, InputType.PerVertex) // Color
        ];

        // Create the pipeline.
        // Pipelines store various pieces of information necessary to draw. All draw commands MUST have a pipeline bound.
        // Here, we pass in our vertex and pixel modules, as well as our input layout.
        // Don't worry about the other values, we'll get into those in later tutorials.
        PipelineDescription pipelineDesc = new PipelineDescription(vertexModule, pixelModule, inputLayout,
            DepthStencilDescription.Disabled, RasterizerDescription.CullNone, null);

        _pipeline = Device.CreatePipeline(pipelineDesc);
    }

    protected override void Draw()
    {
        CommandList.Begin();
        
        // In order to see anything on screen, we must set the viewport before rendering.
        // The viewport tells the GPU how big our render area is. This in turn adjusts where on-screen objects are rendered.
        // Once the viewport is set, it will remain untouched, so we could do this in initialize, but it's easier to do
        // it here. Here we just set it to the window size, as we want to render to the whole window.
        Size windowSize = Window.FramebufferSize;
        CommandList.SetViewport(new Viewport(0, 0, (uint) windowSize.Width, (uint) windowSize.Height));
        
        // Like in the first tutorial, we must begin a render pass before we can draw anything.
        // This tells the GPU which framebuffer we want to render to, and clears it to a color of our choice.
        CommandList.BeginRenderPass(new RenderPassDescription(SwapchainFramebuffer, new Vector4(0.2f, 0.3f, 0.3f, 1.0f)));
        
        // Set our pipeline and buffers.
        // Here, we set the stride to 5 * sizeof(float). 5 is because we pass in 5 values per vertex,
        // 2 for position, and 3 for color. The stride must be in bytes, so we multiply it by the byte size of a float.
        // We use a format of R16_UInt, as our indices are formatted as ushort.
        CommandList.SetPipeline(_pipeline);
        CommandList.SetVertexBuffer(0, _vertexBuffer, 5 * sizeof(float), 0);
        CommandList.SetIndexBuffer(_indexBuffer, Format.R16_UInt);
        
        // Draw our quad to the screen! We tell the GPU we want 6 vertices, as, as mentioned earlier, a quad is made up
        // of 2 right-angled triangles.
        CommandList.DrawIndexed(6);
        
        // End our render pass and finalize the command list.
        CommandList.EndRenderPass();
        CommandList.End();
        
        // Lastly, execute the command list!
        Device.ExecuteCommandList(CommandList);
    }

    public override void Dispose()
    {
        // Remember to dispose everything we've created.
        _pipeline.Dispose();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
        
        base.Dispose();
    }

    public Main() : base("1.2 - Simple Quad") { }
}