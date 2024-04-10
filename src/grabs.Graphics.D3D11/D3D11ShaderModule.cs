using Vortice.Direct3D;

namespace grabs.Graphics.D3D11;

public sealed class D3D11ShaderModule : ShaderModule
{
    public Blob Blob;
    
    public D3D11ShaderModule(ShaderStage stage) : base(stage)
    {
        
    }
    
    public override void Dispose()
    {
        Blob.Dispose();
    }
}