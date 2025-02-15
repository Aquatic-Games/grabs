using System.Diagnostics;
using System.Runtime.CompilerServices;
using grabs.Core;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11CommandList : CommandList
{
    public const int MaxColorAttachments = 8;
    
    private readonly ID3D11RenderTargetView[] _targetCache;
    
    public readonly ID3D11DeviceContext Context;

    public ID3D11CommandList? CommandList;
    
    public D3D11CommandList(ID3D11Device device)
    {
        _targetCache = new ID3D11RenderTargetView[MaxColorAttachments];
        
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
        Debug.Assert(info.ColorAttachments.Length <= MaxColorAttachments);

        int numColorAttachments = info.ColorAttachments.Length;
        for (int i = 0; i < numColorAttachments; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref info.ColorAttachments[i];
            D3D11Texture d3dTexture = (D3D11Texture) attachment.Texture;
            
            Debug.Assert(d3dTexture.RenderTarget != null, "Render Target is null, has the texture been created as a color attachment?");

            _targetCache[i] = d3dTexture.RenderTarget;
            
            if (attachment.LoadOp == LoadOp.Clear)
            {
                ColorF clearColor = attachment.ClearColor;
                Context.ClearRenderTargetView(d3dTexture.RenderTarget,
                    new Color4(clearColor.R, clearColor.G, clearColor.B, clearColor.A));
            }
        }
        
        Context.OMSetRenderTargets(new ReadOnlySpan<ID3D11RenderTargetView>(_targetCache, 0, numColorAttachments));
    }
    
    public override void EndRenderPass() { }
    
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