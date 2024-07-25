using System;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public class GL43Pipeline : Pipeline
{
    private readonly GL _gl;

    public readonly uint VertexProgram;

    public readonly uint FragmentProgram;
    
    public readonly uint Pipeline;
    
    public readonly uint Vao;

    public readonly Silk.NET.OpenGL.PrimitiveType PrimitiveType;

    public readonly DepthStencilDescription DepthStencilDescription;

    public readonly RasterizerDescription RasterizerDescription;

    public readonly BlendDescription BlendDescription;

    public readonly GL43DescriptorLayout[] Layouts;

    public GL43Pipeline(GL gl, in PipelineDescription description)
    {
        _gl = gl;

        GL43ShaderModule vShaderModule = (GL43ShaderModule) description.VertexShader;
        GL43ShaderModule pShaderModule = (GL43ShaderModule) description.PixelShader;

        VertexProgram = CreateShaderProgram(gl, vShaderModule);
        FragmentProgram = CreateShaderProgram(gl, pShaderModule);

        Pipeline = _gl.GenProgramPipeline();
        _gl.UseProgramStages(Pipeline, UseProgramStageMask.VertexShaderBit, VertexProgram);
        _gl.UseProgramStages(Pipeline, UseProgramStageMask.FragmentShaderBit, FragmentProgram);

        Vao = _gl.CreateVertexArray();
        _gl.BindVertexArray(Vao);

        for (uint i = 0; i < description.InputLayout?.Length; i++)
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
                case Format.R8G8B8A8_UNorm:
                    _gl.VertexAttribFormat(i, 4, VertexAttribType.UnsignedByte, true, desc.Offset);
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
        BlendDescription = description.BlendState;

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

        if (description.DescriptorLayouts != null)
        {
            Layouts = new GL43DescriptorLayout[description.DescriptorLayouts.Length];
            uint currentBindIndex = 0;
            for (int i = 0; i < Layouts.Length; i++)
            {
                GL43DescriptorLayout layout = (GL43DescriptorLayout) description.DescriptorLayouts[i];
                DescriptorBindingDescription[] bindings = new DescriptorBindingDescription[layout.Bindings.Length];

                for (int j = 0; j < bindings.Length; j++)
                {
                    DescriptorBindingDescription desc = layout.Bindings[j];
                    desc.Binding = currentBindIndex++;
                    bindings[j] = desc;
                }

                Layouts[i] = new GL43DescriptorLayout(bindings);
            }
        }
    }
    
    public override void Dispose()
    {
        _gl.DeleteVertexArray(Vao);
        
        _gl.DeleteProgramPipeline(Pipeline);
        _gl.DeleteProgram(FragmentProgram);
        _gl.DeleteProgram(VertexProgram);
    }

    private static uint CreateShaderProgram(GL gl, GL43ShaderModule module)
    {
        uint program = gl.CreateProgram();
        gl.AttachShader(program, module.Shader);
        gl.ProgramParameter(program, ProgramParameterPName.Separable, (int) GLEnum.True);
        
        gl.LinkProgram(program);
        gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to link program: {gl.GetProgramInfoLog(program)}");
        
        gl.DetachShader(program, module.Shader);

        return program;
    }
}