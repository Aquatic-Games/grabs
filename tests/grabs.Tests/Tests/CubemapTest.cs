using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using grabs.Tests.Utils;
using StbImageSharp;
using Buffer = grabs.Graphics.Buffer;

namespace grabs.Tests.Tests;

public class CubemapTest : TestBase
{
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Pipeline _pipeline;

    private Matrix4x4 _transformMatrix;
    
    private Buffer _cameraBuffer;
    private Buffer _worldBuffer;
    private Texture _cubemap;
    private DescriptorSet _set;
    
    protected override void Initialize()
    {
        base.Initialize();

        Cube cube = new Cube();

        _vertexBuffer = Device.CreateBuffer(BufferType.Vertex, cube.Vertices);
        _indexBuffer = Device.CreateBuffer(BufferType.Index, cube.Indices);

        string hlsl = File.ReadAllText("Shaders/Cubemap.hlsl");
        byte[] vSpv = Compiler.CompileToSpirV(hlsl, "VSMain", ShaderStage.Vertex, true);
        byte[] pSpv = Compiler.CompileToSpirV(hlsl, "PSMain", ShaderStage.Pixel, true);

        using ShaderModule vModule = Device.CreateShaderModule(ShaderStage.Vertex, vSpv, "VSMain");
        using ShaderModule pModule = Device.CreateShaderModule(ShaderStage.Pixel, pSpv, "PSMain");

        InputLayoutDescription[] inputLayout =
            [new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex)];

        using DescriptorLayout descriptor = Device.CreateDescriptorLayout(
            new DescriptorLayoutDescription(
                new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex),
                new DescriptorBindingDescription(1, DescriptorType.ConstantBuffer, ShaderStage.Vertex),
                new DescriptorBindingDescription(2, DescriptorType.Image, ShaderStage.Pixel)));

        PipelineDescription pipelineDesc = new PipelineDescription(vModule, pModule, inputLayout,
            DepthStencilDescription.DepthLessEqual, RasterizerDescription.CullClockwise, BlendDescription.Disabled,
            [descriptor]);

        _pipeline = Device.CreatePipeline(pipelineDesc);

        CameraMatrices matrices = new CameraMatrices()
        {
            Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4,
                SizeInPixels.Width / (float) SizeInPixels.Height, 0.1f, 100.0f),
            View = Matrix4x4.CreateLookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY)
        };

        _transformMatrix = Matrix4x4.Identity;
        
        _cameraBuffer = Device.CreateBuffer(BufferType.Constant, matrices);
        _worldBuffer = Device.CreateBuffer(BufferType.Constant, _transformMatrix, true);
        
        ImageResult right = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\right.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult left = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\left.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult top = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\top.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult bottom = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\bottom.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult front = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\front.jpg"), ColorComponents.RedGreenBlueAlpha);
        ImageResult back = ImageResult.FromMemory(File.ReadAllBytes(@"C:\Users\ollie\Pictures\skybox\back.jpg"), ColorComponents.RedGreenBlueAlpha);

        _cubemap = Device.CreateTexture(
            TextureDescription.Cubemap((uint) right.Width, (uint) right.Height, 1, Format.R8G8B8A8_UNorm,
                TextureUsage.ShaderResource), [right.Data, left.Data, top.Data, bottom.Data, front.Data, back.Data]);

        _set = Device.CreateDescriptorSet(descriptor, 
            new DescriptorSetDescription(buffer: _cameraBuffer),
            new DescriptorSetDescription(buffer: _worldBuffer),
            new DescriptorSetDescription(texture: _cubemap));
    }

    protected override void Update(float dt)
    {
        base.Update(dt);
        
        _transformMatrix *= Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, dt) *
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, dt);
    }

    protected override void Draw()
    {
        CommandList.Begin();
        CommandList.SetViewport(new Viewport(0, 0, 1280, 720));
        CommandList.SetScissor(new Rectangle(0, 0, 1280, 720));
        
        CommandList.UpdateBuffer(_worldBuffer, 0, _transformMatrix);

        CommandList.BeginRenderPass(new RenderPassDescription(Framebuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
        
        CommandList.SetPipeline(_pipeline);
        CommandList.SetDescriptorSet(0, _set);
        
        CommandList.SetVertexBuffer(0, _vertexBuffer, Vertex.SizeInBytes, 0);
        CommandList.SetIndexBuffer(_indexBuffer, Format.R16_UInt);
        
        CommandList.DrawIndexed(36);
        
        CommandList.EndRenderPass();
        
        CommandList.End();
        Device.ExecuteCommandList(CommandList);
    }

    public override void Dispose()
    {
        _cubemap.Dispose();
        
        base.Dispose();
    }
    
    private struct CameraMatrices
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
    }

    public CubemapTest() : base("Skybox Test") { }
}