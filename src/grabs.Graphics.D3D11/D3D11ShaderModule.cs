using grabs.SpirvTools;
using Vortice.D3DCompiler;
using Vortice.Direct3D;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11ShaderModule : ShaderModule
{
    public readonly Blob Blob;

    public D3D11ShaderModule(ShaderStage stage, ReadOnlySpan<byte> spirv, string entryPoint)
    {
        string hlsl = CompilerUtils.HlslFromSpirv(stage, in spirv, entryPoint);

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_5_0",
            ShaderStage.Pixel => "ps_5_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        if (Compiler.Compile(hlsl, "main", null, profile, out Blob, out Blob errorBlob).Failure)
            throw new Exception($"Failed to compile HLSL: {errorBlob.AsString()}");
    }
    
    public override void Dispose()
    {
        Blob.Dispose();
    }
}