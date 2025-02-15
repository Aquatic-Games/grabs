using System.Diagnostics;
using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11CommandList : CommandList
{
    public readonly ID3D11DeviceContext Context;

    public ID3D11CommandList? CommandList;
    
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
        Debug.Assert(CommandList == null, "Command List was not null, have you called Begin()?");
        
        CommandList = Context.FinishCommandList(false);
    }
    
    public override void BeginRenderPass(in RenderPassInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override void EndRenderPass()
    {
        throw new NotImplementedException();
    }
    
    public override void SetViewport(in Viewport viewport)
    {
        throw new NotImplementedException();
    }
    
    public override void SetPipeline(Pipeline pipeline)
    {
        throw new NotImplementedException();
    }
    
    public override void SetVertexBuffer(uint slot, Buffer vertexBuffer, ulong offset = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void SetIndexBuffer(Buffer indexBuffer, Format format, ulong offset = 0)
    {
        throw new NotImplementedException();
    }
    
    public override void Draw(uint numVertices)
    {
        throw new NotImplementedException();
    }
    
    public override void DrawIndexed(uint numIndices)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        CommandList?.Dispose();
        Context.Dispose();
    }
}