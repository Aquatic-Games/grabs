using Vortice.Direct3D;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11ShaderModule : ShaderModule
{
    public readonly Blob Blob;

    public D3D11ShaderModule(ShaderStage stage, ReadOnlySpan<byte> spirv, string entryPoint)
    {
        
    }
    
    public override void Dispose()
    {
        Blob.Dispose();
    }
}