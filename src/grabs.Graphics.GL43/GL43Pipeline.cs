using System;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public class GL43Pipeline : Pipeline
{
    private readonly GL _gl;

    public readonly uint ShaderProgram;
    
    public readonly uint Vao;

    public readonly Silk.NET.OpenGL.PrimitiveType PrimitiveType;

    public readonly DepthStencilDescription DepthStencilDescription;

    public readonly RasterizerDescription RasterizerDescription;

    public GL43Pipeline(GL gl, in PipelineDescription description)
    {
        _gl = gl;

        GL43ShaderModule vShaderModule = (GL43ShaderModule) description.VertexShader;
        GL43ShaderModule pShaderModule = (GL43ShaderModule) description.PixelShader;
        
        ShaderProgram = _gl.CreateProgram();
        _gl.AttachShader(ShaderProgram, vShaderModule.Shader);
        _gl.AttachShader(ShaderProgram, pShaderModule.Shader);
        
        _gl.LinkProgram(ShaderProgram);

        _gl.GetProgram(ShaderProgram, ProgramPropertyARB.LinkStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to link program: {_gl.GetProgramInfoLog(ShaderProgram)}");
        
        _gl.DetachShader(ShaderProgram, vShaderModule.Shader);
        _gl.DetachShader(ShaderProgram, pShaderModule.Shader);

        Vao = _gl.CreateVertexArray();
        _gl.BindVertexArray(Vao);

        for (uint i = 0; i < description.InputLayout.Length; i++)
        {
            _gl.EnableVertexAttribArray(i);
            
            InputLayoutDescription desc = description.InputLayout[i];

            // TODO: THE REST OF THESE!!!
            switch (desc.Format)
            {
                case Format.R32_Float:
                    _gl.VertexAttribFormat(i, 1, VertexAttribType.Float, false, desc.Offset);
                    break;
                case Format.R32G32_Float:
                    _gl.VertexAttribFormat(i, 2, VertexAttribType.Float, false, desc.Offset);
                    break;
                case Format.R32G32B32_Float:
                    _gl.VertexAttribFormat(i, 3, VertexAttribType.Float, false, desc.Offset);
                    break;
                case Format.R32G32B32A32_Float:
                    _gl.VertexAttribFormat(i, 4, VertexAttribType.Float, false, desc.Offset);
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            // TODO: This doesn't work with instancing - this needs to be reworked when instancing is done.
            _gl.VertexBindingDivisor(i, (uint) desc.Type);
            _gl.VertexAttribBinding(i, 0);
        }

        DepthStencilDescription = description.DepthStencilState;
        RasterizerDescription = description.RasterizerState;

        PrimitiveType = description.PrimitiveType switch
        {
            Graphics.PrimitiveType.PointList => Silk.NET.OpenGL.PrimitiveType.Points,
            Graphics.PrimitiveType.LineList => Silk.NET.OpenGL.PrimitiveType.Lines,
            Graphics.PrimitiveType.LineStrip => Silk.NET.OpenGL.PrimitiveType.LineStrip,
            Graphics.PrimitiveType.LineListAdjacency => Silk.NET.OpenGL.PrimitiveType.LinesAdjacency,
            Graphics.PrimitiveType.LineStripAdjacency => Silk.NET.OpenGL.PrimitiveType.LineStripAdjacency,
            Graphics.PrimitiveType.TriangleList => Silk.NET.OpenGL.PrimitiveType.Triangles,
            Graphics.PrimitiveType.TriangleStrip => Silk.NET.OpenGL.PrimitiveType.TriangleStrip,
            Graphics.PrimitiveType.TriangleFan => Silk.NET.OpenGL.PrimitiveType.TriangleFan,
            Graphics.PrimitiveType.TriangleListAdjacency => Silk.NET.OpenGL.PrimitiveType.TrianglesAdjacency,
            Graphics.PrimitiveType.TriangleStripAdjacency => Silk.NET.OpenGL.PrimitiveType.TriangleStripAdjacency,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public override void Dispose()
    {
        _gl.DeleteProgram(ShaderProgram);
        _gl.DeleteVertexArray(Vao);
    }
}