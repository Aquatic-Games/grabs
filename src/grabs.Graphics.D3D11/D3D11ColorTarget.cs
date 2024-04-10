using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

public class D3D11ColorTarget : ColorTarget
{
    public ID3D11RenderTargetView RenderTarget;
    
    public D3D11ColorTarget(ID3D11RenderTargetView renderTarget)
    {
        RenderTarget = renderTarget;
    }
    
    public override void Dispose()
    {
        RenderTarget.Dispose();
    }
}