using System;
using System.Runtime.CompilerServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace grabs.Graphics.D3D11;

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
        D3D11Framebuffer framebuffer = (D3D11Framebuffer) description.Framebuffer;
        
        Context.OMSetRenderTargets(framebuffer.RenderTargets, framebuffer.DepthTarget);

        switch (description.LoadOp)
        {
            case LoadOp.Clear:
                for (int i = 0; i < framebuffer.RenderTargets.Length; i++)
                    Context.ClearRenderTargetView(framebuffer.RenderTargets[i], new Color4(description.ClearColor));
                break;

            case LoadOp.Load:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void EndRenderPass() { }

    public override unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;

        if (d3dBuffer.Description.Dynamic)
        {
            MappedSubresource mResource = Context.Map(d3dBuffer.Buffer, MapMode.WriteDiscard);
            Unsafe.CopyBlock((byte*) mResource.DataPointer + offsetInBytes, pData, sizeInBytes);
            Context.Unmap(d3dBuffer.Buffer);
        }
        else
        {
            Context.UpdateSubresource(d3dBuffer.Buffer, 0,
                new Box((int) offsetInBytes, 0, 0, (int) (offsetInBytes + sizeInBytes), 1, 1), (nint) pData, 0, 0);
        }
    }

    public override void SetViewport(in Viewport viewport)
    {
        Context.RSSetViewport(viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinDepth,
            viewport.MaxDepth);
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        
        Context.VSSetShader(d3dPipeline.VertexShader);
        Context.PSSetShader(d3dPipeline.PixelShader);
        Context.IASetInputLayout(d3dPipeline.InputLayout);
        
        // TODO: Add these to pipelines.
        Context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset)
    {
        Context.IASetVertexBuffer((int) slot, ((D3D11Buffer) buffer).Buffer, (int) stride, (int) offset);
    }

    public override void SetIndexBuffer(Buffer buffer, Format format)
    {
        Context.IASetIndexBuffer(((D3D11Buffer) buffer).Buffer, format.ToDXGIFormat(), 0);
    }

    public override void SetConstantBuffer(uint slot, Buffer buffer)
    {
        Context.VSSetConstantBuffer((int) slot, ((D3D11Buffer) buffer).Buffer);
    }

    public override void DrawIndexed(uint numIndices)
    {
        Context.DrawIndexed((int) numIndices, 0, 0);
    }

    public override void Dispose()
    {
        CommandList?.Dispose();
        Context.Dispose();
    }
}