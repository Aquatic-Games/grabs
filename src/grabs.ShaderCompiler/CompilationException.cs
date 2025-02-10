namespace grabs.ShaderCompiler;

public class CompilationException(ShaderStage stage, string message) : Exception($"Failed to compile {stage} shader: {message}");