using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using ColorF = grabs.Core.ColorF;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11CommandList : CommandList
{
    public override bool IsDisposed { get; protected set; }

    private readonly ID3D11DeviceContext* _context;

    public ID3D11CommandList* CommandList;

    public D3D11CommandList(ID3D11Device* device)
    {
        GrabsLog.Log("Creating deferred context.");
        fixed (ID3D11DeviceContext** context = &_context)
            device->CreateDeferredContext(0, context).Check("Create deferred context");

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
        Debug.Assert(CommandList == null);

        fixed (ID3D11CommandList** list = &CommandList)
            _context->FinishCommandList(false, list).Check("Finish command list");
    }
    
    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        ID3D11RenderTargetView** colorTargets = stackalloc ID3D11RenderTargetView*[colorAttachments.Length];

        for (int i = 0; i < colorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref colorAttachments[i];
            D3D11Texture texture = (D3D11Texture) attachment.Texture;
            colorTargets[i] = texture.RenderTarget;
        }

        // It's seemingly more efficient to the driver to set, then clear, even though it requires 2 for loops.
        _context->OMSetRenderTargets((uint) colorAttachments.Length, colorTargets, null);

        for (int i = 0; i < colorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref colorAttachments[i];
            D3D11Texture texture = (D3D11Texture) attachment.Texture;

            if (attachment.LoadOp == LoadOp.Clear)
            {
                ColorF clearColor = attachment.ClearColor;
                _context->ClearRenderTargetView(texture.RenderTarget, &clearColor.R);
            }
        }

        D3D11_VIEWPORT viewport = new()
        {
            TopLeftX = 0,
            TopLeftY = 0,
            Width = colorAttachments[0].Texture.Size.Width,
            Height = colorAttachments[0].Texture.Size.Height,
            MinDepth = 0,
            MaxDepth = 1
        };
        _context->RSSetViewports(1, &viewport);
    }
    
    public override void EndRenderPass() { }
    
    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;

        _context->VSSetShader(d3dPipeline.VertexShader, null, 0);
        _context->PSSetShader(d3dPipeline.PixelShader, null, 0);
        _context->IASetPrimitiveTopology(d3dPipeline.PrimitiveTopology);
    }
    
    public override void Draw(uint numVertices)
    {
        _context->Draw(numVertices, 0);
    }
    
    public override void Dispose()
    {
        if (CommandList != null)
            CommandList->Release();

        _context->Release();
    }
}