using System;
using System.IO;
using System.Numerics;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using grabs.Tests.Utils;
using Buffer = grabs.Graphics.Buffer;

namespace grabs.Tests.Tests;

public class CubeTest : TestBase
{
    private Buffer _vertexBuffer;
    private Buffer _indexBuffer;

    private Buffer _cameraBuffer;
    private Buffer _transformBuffer;

    private Pipeline _pipeline;
    
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
            View = Matrix4x4.CreateLookAt(new Vector3(0, 0, -3), Vector3.Zero, Vector3.One)
        };
        
        _cameraBuffer = Device.CreateBuffer(BufferType.Constant, matrices);
        _transformBuffer = Device.CreateBuffer(BufferType.Constant, Matrix4x4.Identity, true);

        string hlsl = File.ReadAllText("Shaders/Basic3D.hlsl");
        byte[] vSpv = Compiler.CompileToSpirV(hlsl, "Vertex", ShaderStage.Vertex, true);
        byte[] pSpv = Compiler.CompileToSpirV(hlsl, "Pixel", ShaderStage.Pixel, true);

        using ShaderModule vModule = Device.CreateShaderModule(ShaderStage.Vertex, vSpv, "Vertex");
        using ShaderModule pModule = Device.CreateShaderModule(ShaderStage.Pixel, pSpv, "Pixel");

        _pipeline = Device.CreatePipeline(new PipelineDescription(vModule, pModule, new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // Position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex) // TexCoord
        }, DepthStencilDescription.Disabled));
    }

    public override void Dispose()
    {
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