using Silk.NET.Shaderc;

namespace grabs.Graphics.Internal;

internal static class CompilerUtils
{
    private static readonly Shaderc _shaderc;

    static CompilerUtils()
    {
        _shaderc = Shaderc.GetApi();
    }
    
    public static string HlslFromSpirv(ShaderStage stage, in ReadOnlySpan<byte> spirv, string entryPoint)
    {
        return "";
    }
}