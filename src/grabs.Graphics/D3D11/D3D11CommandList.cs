using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TerraFX.Interop.DirectX;
using static grabs.Graphics.D3D11.D3D11Result;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11CommandList : CommandList
{
    public readonly ID3D11DeviceContext* Context;

    public ID3D11CommandList* CommandList;

    public D3D11CommandList(ID3D11Device* device)
    {
        fixed (ID3D11DeviceContext** context = &Context)
            CheckResult(device->CreateDeferredContext(0, context), "Create deferred context");

        CommandList = null;
    }
    
    public override void Begin()
    {
        if (CommandList != null)
            CommandList->Release();
        
        CommandList = null;
    }

    public override void End()
    {
        fixed (ID3D11CommandList** commandList = &CommandList)
            CheckResult(Context->FinishCommandList(false, commandList), "Finish command list");
    }

    public override void BeginRenderPass(in RenderPassDescription description)
    {
        int numRenderTargets = description.ColorAttachments.Length;
        
        ID3D11RenderTargetView** renderTargets = stackalloc ID3D11RenderTargetView*[numRenderTargets];
        
        for (int i = 0; i < numRenderTargets; i++)
        {
            ref readonly ColorAttachmentDescription desc = ref description.ColorAttachments[i];
            D3D11Texture texture = (D3D11Texture) desc.Texture;
            renderTargets[i] = texture.RenderTarget;
            
            if (desc.LoadOp == LoadOp.Clear)
            {
                Color4 color = desc.ClearColor;
                Context->ClearRenderTargetView(texture.RenderTarget, (float*) Unsafe.AsPointer(ref color));
            }
        }

        Context->OMSetRenderTargets((uint) numRenderTargets, renderTargets, null);
    }

    public override void EndRenderPass()
    {
        // Does nothing
    }

    public override void SetViewport(in Viewport viewport)
    {
        D3D11_VIEWPORT d3dViewport = new()
        {
            TopLeftX = viewport.X,
            TopLeftY = viewport.Y,
            Width = viewport.Width,
            Height = viewport.Height,
            MinDepth = viewport.MinDepth,
            MaxDepth = viewport.MaxDepth
        };

        Context->RSSetViewports(1, &d3dViewport);
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;

        Context->VSSetShader(d3dPipeline.VertexShader, null, 0);
        Context->PSSetShader(d3dPipeline.PixelShader, null, 0);
        Context->IASetInputLayout(d3dPipeline.InputLayout);
        Context->IASetPrimitiveTopology(d3dPipeline.PrimitiveTopology);
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        ID3D11Buffer* buf = d3dBuffer.Buffer;

        Context->IASetVertexBuffers(slot, 1, &buf, &stride, &offset);
    }

    public override void SetIndexBuffer(Buffer buffer, Format format)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;

        Context->IASetIndexBuffer(d3dBuffer.Buffer, format.ToD3D(), 0);
    }

    public override void Draw(uint numVertices)
    {
        Context->Draw(numVertices, 0);
    }

    public override void DrawIndexed(uint numIndices)
    {
        Context->DrawIndexed(numIndices, 0, 0);
    }

    public override void Dispose()
    {
        if (CommandList != null)
            CommandList->Release();
        
        CommandList = null;
        Context->Release();
    }
}