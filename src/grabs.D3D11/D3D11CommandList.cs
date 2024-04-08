using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace grabs.D3D11;

public sealed class D3D11CommandList : CommandList
{
    public ID3D11DeviceContext Context;
    public ID3D11CommandList CommandList;

    public D3D11CommandList(ID3D11Device device)
    {
        Context = device.CreateDeferredContext();
    }

    public override void Begin()
    {
        CommandList?.Dispose();
        CommandList = null;
    }

    public override void End()
    {
        CommandList = Context.FinishCommandList(false);
    }

    public override void BeginRenderPass(in RenderPassDescription description)
    {
        for (int i = 0; i < description.ColorTargets.Length; i++)
        {
            D3D11ColorTarget target = (D3D11ColorTarget) description.ColorTargets[i];
            
            Context.ClearRenderTargetView(target.RenderTarget, new Color4(description.ClearColor));
        }
    }

    public override void EndRenderPass()
    {
        
    }

    public override void Dispose()
    {
        CommandList?.Dispose();
        Context.Dispose();
    }
}