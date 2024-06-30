using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using Common;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using grabs.Utils;
using StbImageSharp;
using Buffer = grabs.Graphics.Buffer;

namespace Tutorial;

public class Main : SampleBase
{
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Pipeline _pipeline;

    private Texture _texture1;
    private Texture _texture2;
    private DescriptorSet _descriptorSet;

    protected override void Initialize()
    {
        VertexPositionTexture[] vertices = new VertexPositionTexture[]
        {
            new VertexPositionTexture(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 1))
        };

        ushort[] indices = new ushort[]
        {
            0, 1, 3,
            1, 2, 3
        };

        _vertexBuffer = Device.CreateBuffer(BufferType.Vertex, vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.Index, indices);

        string shader = File.ReadAllText("Shader.hlsl");

        byte[] vertexSpirv = Compiler.CompileToSpirV(shader, "VSMain", ShaderStage.Vertex, true);
        byte[] pixelSpirv = Compiler.CompileToSpirV(shader, "PSMain", ShaderStage.Pixel, true);

        using ShaderModule vertexModule = Device.CreateShaderModule(ShaderStage.Vertex, vertexSpirv, "VSMain");
        using ShaderModule pixelModule = Device.CreateShaderModule(ShaderStage.Pixel, pixelSpirv, "PSMain");

        InputLayoutDescription[] inputLayout =
        [
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex),
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex)
        ];

        DescriptorLayoutDescription descriptorDesc = new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.Texture, ShaderStage.Pixel),
            new DescriptorBindingDescription(1, DescriptorType.Texture, ShaderStage.Pixel)
        );

        using DescriptorLayout layout = Device.CreateDescriptorLayout(descriptorDesc);

        PipelineDescription pipelineDesc = new PipelineDescription(vertexModule, pixelModule, inputLayout,
            DepthStencilDescription.Disabled, RasterizerDescription.CullNone, BlendDescription.Disabled, [layout]);

        _pipeline = Device.CreatePipeline(pipelineDesc);
        
        ImageResult result1 = ImageResult.FromMemory(File.ReadAllBytes("Content/container.png"), ColorComponents.RedGreenBlueAlpha);
        ImageResult result2 = ImageResult.FromMemory(File.ReadAllBytes("Content/awesomeface.png"), ColorComponents.RedGreenBlueAlpha);
        
        TextureDescription texture1Desc = TextureDescription.Texture2D((uint) result1.Width, (uint) result1.Height, 1, Format.R8G8B8A8_UNorm, TextureUsage.ShaderResource);
        _texture1 = Device.CreateTexture(texture1Desc, result1.Data);
        
        TextureDescription texture2Desc = TextureDescription.Texture2D((uint) result2.Width, (uint) result2.Height, 1, Format.R8G8B8A8_UNorm, TextureUsage.ShaderResource);
        _texture2 = Device.CreateTexture(texture2Desc, result2.Data);

        DescriptorSetDescription[] descriptors =
        [
            new DescriptorSetDescription(texture: _texture1),
            new DescriptorSetDescription(texture: _texture2)
        ];
        
        _descriptorSet = Device.CreateDescriptorSet(layout, descriptors);
    }

    protected override void Draw()
    {
        CommandList.Begin();

        Size windowSize = Window.FramebufferSize;
        CommandList.SetViewport(new Viewport(0, 0, (uint) windowSize.Width, (uint) windowSize.Height));
        
        CommandList.BeginRenderPass(new RenderPassDescription(SwapchainFramebuffer, new Vector4(0.2f, 0.3f, 0.3f, 1.0f)));
        
        CommandList.SetPipeline(_pipeline);
        CommandList.SetDescriptorSet(0, _descriptorSet);
        
        CommandList.SetVertexBuffer(0, _vertexBuffer, VertexPositionTexture.SizeInBytes, 0);
        CommandList.SetIndexBuffer(_indexBuffer, Format.R16_UInt);
        
        CommandList.DrawIndexed(6);
        
        CommandList.EndRenderPass();
        CommandList.End();
        
        Device.ExecuteCommandList(CommandList);
    }

    public Main() : base("Tutorial 1.3 - Texturing") { }
}