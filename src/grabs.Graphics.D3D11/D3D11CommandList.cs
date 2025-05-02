using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;

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
        throw new NotImplementedException();
    }
    
    public override void EndRenderPass()
    {
        throw new NotImplementedException();
    }
    
    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        throw new NotImplementedException();
    }
    
    public override void Draw(uint numVertices)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        if (CommandList != null)
            CommandList->Release();

        _context->Release();
    }
}