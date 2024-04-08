using Vortice.Direct3D11;

namespace grabs.D3D11;

public class D3D11CommandList : CommandList
{
    public ID3D11DeviceContext Context;
    public ID3D11CommandList CommandList;

    public D3D11CommandList(ID3D11Device device)
    {
        Context = device.CreateDeferredContext();
    }

    public override void Begin()
    {
        throw new NotImplementedException();
    }

    public override void End()
    {
        throw new NotImplementedException();
    }

    public override void BeginRenderPass(in RenderPassDescription description)
    {
        throw new NotImplementedException();
    }

    public override void EndRenderPass()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        Context.Dispose();
    }
}