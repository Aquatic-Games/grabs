namespace grabs.Graphics.Exceptions;

public class ShaderCompilationException : Exception
{
    public ShaderCompilationException(ShaderStage stage, string error) : base(
        $"{stage} shader failed to compile: {error}") { }
}