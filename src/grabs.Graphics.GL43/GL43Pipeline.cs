using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public class GL43Pipeline : Pipeline
{
    private GL _gl;

    public uint ShaderProgram;
    
    public uint Vao;

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
        
        
    }
    
    public override void Dispose()
    {
        _gl.DeleteProgram(ShaderProgram);
        _gl.DeleteVertexArray(Vao);
    }
}