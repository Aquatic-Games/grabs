using System;
using System.IO;
using System.Numerics;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using grabs.Tests.Utils;
using StbImageSharp;
using Buffer = grabs.Graphics.Buffer;

namespace grabs.Tests.Tests;

public class CubeTest : TestBase
{
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Buffer _cameraBuffer;
    private Buffer _transformBuffer;

    private Pipeline _pipeline;

    private Texture _texture;

    private DescriptorSet _transformSet;
    private DescriptorSet _textureSet;

    private Matrix4x4 _transformMatrix;
    
    public CubeTest() : base("Cube Test") { }

    protected override void Initialize()
    {
        base.Initialize();

        Cube cube = new Cube();

        _vertexBuffer = Device.CreateBuffer(BufferType.Vertex, new ReadOnlySpan<Vertex>(cube.Vertices));
        _indexBuffer = Device.CreateBuffer(BufferType.Index, new ReadOnlySpan<ushort>(cube.Indices));

        CameraMatrices matrices = new CameraMatrices()
        {
            Projection = Matrix4x4.CreatePerspectiveFieldOfView(45 * float.Pi / 180, 1280 / 720f, 0.1f, 100f),
            View = Matrix4x4.CreateLookAt(new Vector3(0, 0, -3), Vector3.Zero, Vector3.UnitY)
        };
        
        _cameraBuffer = Device.CreateBuffer(BufferType.Constant, matrices);
        
        _transformMatrix = Matrix4x4.Identity;
        _transformBuffer = Device.CreateBuffer(BufferType.Constant, _transformMatrix, true);

        DescriptorLayoutDescription transformLayoutDesc = new DescriptorLayoutDescription(
            new DescriptorBindingDescription(0, DescriptorType.ConstantBuffer, ShaderStage.Vertex),
            new DescriptorBindingDescription(1, DescriptorType.ConstantBuffer, ShaderStage.Vertex));

        DescriptorLayoutDescription textureLayoutDesc =
            new DescriptorLayoutDescription(new DescriptorBindingDescription(0, DescriptorType.Texture,
                ShaderStage.Pixel));

        DescriptorLayout transformLayout = Device.CreateDescriptorLayout(transformLayoutDesc);
        DescriptorLayout textureLayout = Device.CreateDescriptorLayout(textureLayoutDesc);

        string hlsl = File.ReadAllText("Shaders/Basic3D.hlsl");
        byte[] vSpv = Compiler.CompileToSpirV(hlsl, "Vertex", ShaderStage.Vertex, true);
        byte[] pSpv = Compiler.CompileToSpirV(hlsl, "Pixel", ShaderStage.Pixel, true);

        using ShaderModule vModule = Device.CreateShaderModule(ShaderStage.Vertex, vSpv, "Vertex");
        using ShaderModule pModule = Device.CreateShaderModule(ShaderStage.Pixel, pSpv, "Pixel");

        _pipeline = Device.CreatePipeline(new PipelineDescription(vModule, pModule, new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex) // TexCoord
        }, DepthStencilDescription.DepthLessEqual, RasterizerDescription.CullClockwise, [transformLayout, textureLayout]));

        using FileStream stream = File.OpenRead("Assets/awesomeface.png");
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        
        _texture = Device.CreateTexture(TextureDescription.Texture2D((uint) result.Width, (uint) result.Height, 0,
            Format.R8G8B8A8_UNorm, TextureUsage.ShaderResource | TextureUsage.GenerateMips), new ReadOnlySpan<byte>(result.Data));
        
        CommandList.Begin();
        CommandList.GenerateMipmaps(_texture);
        CommandList.End();
        Device.ExecuteCommandList(CommandList);

        _transformSet = Device.CreateDescriptorSet(transformLayout, new DescriptorSetDescription(buffer: _cameraBuffer),
            new DescriptorSetDescription(buffer: _transformBuffer));

        _textureSet = Device.CreateDescriptorSet(textureLayout, new DescriptorSetDescription(texture: _texture));
        
        transformLayout.Dispose();
        textureLayout.Dispose();
    }

    protected override void Update(float dt)
    {
        base.Update(dt);

        _transformMatrix *= Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, dt) *
                            Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, dt);
    }

    protected override void Draw()
    {
        base.Draw();
        
        CommandList.Begin();
        
        CommandList.UpdateBuffer(_transformBuffer, 0, _transformMatrix);

        CommandList.BeginRenderPass(new RenderPassDescription(Framebuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
        CommandList.SetViewport(new Viewport(0, 0, 1280, 720));
        
        CommandList.SetPipeline(_pipeline);
        CommandList.SetVertexBuffer(0, _vertexBuffer, Vertex.SizeInBytes, 0);
        CommandList.SetIndexBuffer(_indexBuffer, Format.R16_UInt);
        
        CommandList.SetDescriptorSet(0, _transformSet);
        CommandList.SetDescriptorSet(1, _textureSet);
        
        CommandList.DrawIndexed(36);
        
        CommandList.EndRenderPass();
        
        CommandList.End();
        
        Device.ExecuteCommandList(CommandList);
    }

    public override void Dispose()
    {
        _transformSet.Dispose();
        _textureSet.Dispose();
        
        _texture.Dispose();
        
        _pipeline.Dispose();
        
        _transformBuffer.Dispose();
        _cameraBuffer.Dispose();
        
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
        
        base.Dispose();
    }

    private struct CameraMatrices
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
    }
}