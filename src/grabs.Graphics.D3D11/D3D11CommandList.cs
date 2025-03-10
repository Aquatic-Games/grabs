using System.Diagnostics;
using grabs.Core;
using Vortice.Direct3D;
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
        Vortice.Mathematics.Viewport d3dViewport = new Vortice.Mathematics.Viewport()
        {
            X = viewport.X,
            Y = viewport.Y,
            Width = viewport.Width,
            Height = viewport.Height,
            MinDepth = viewport.MinDepth,
            MaxDepth = viewport.MaxDepth
        };
        
        Context.RSSetViewport(d3dViewport);
    }
    
    public override void SetPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        
        Context.VSSetShader(d3dPipeline.VertexShader);
        Context.PSSetShader(d3dPipeline.PixelShader);
        Context.IASetInputLayout(d3dPipeline.Layout);
        Context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
    }
    
    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        
        Context.IASetVertexBuffer(slot, d3dBuffer.Buffer, stride, offset);
    }
    
    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        
        Context.IASetIndexBuffer(d3dBuffer.Buffer, format.ToD3D(), offset);
    }

    public override void PushDescriptors(uint slot, Pipeline pipeline, in ReadOnlySpan<Descriptor> descriptors)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        
        foreach (Descriptor descriptor in descriptors)
        {
            switch (descriptor.Type)
            {
                // TODO: This code assumes that the CB will be in the vtx shader and the Texture will be in the pix shader.
                case DescriptorType.ConstantBuffer:
                {
                    Debug.Assert(descriptor.Buffer != null);
                    Dictionary<uint, uint> remapping = d3dPipeline.VertexRemappings[slot];
                    Context.VSSetConstantBuffer(remapping[descriptor.Slot], ((D3D11Buffer) descriptor.Buffer).Buffer);
                    break;
                }
                case DescriptorType.Texture:
                {
                    Debug.Assert(descriptor.Texture != null);
                    Dictionary<uint, uint> remapping = d3dPipeline.PixelRemappings[slot];
                    Context.PSSetShaderResource(remapping[descriptor.Slot], ((D3D11Texture) descriptor.Texture).ResourceView);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Draw(uint numVertices)
    {
        Context.Draw(numVertices, 0);
    }
    
    public override void DrawIndexed(uint numIndices)
    {
        Context.DrawIndexed(numIndices, 0, 0);
    }
    
    public override void Dispose()
    {
        CommandList?.Dispose();
        Context.Dispose();
    }
}