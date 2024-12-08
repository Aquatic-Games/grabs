using System.Diagnostics.CodeAnalysis;
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

    public override void Dispose()
    {
        if (CommandList != null)
            CommandList->Release();
        
        CommandList = null;
        Context->Release();
    }
}