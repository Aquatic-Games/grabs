using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Texture : Texture
{
    public override bool IsDisposed { get; protected set; }

    private readonly ID3D11Resource* _texture;

    public readonly ID3D11RenderTargetView* RenderTarget;

    public D3D11Texture(ID3D11Device* device, ID3D11Texture2D* texture, Size2D size) : base(size)
    {
        _texture = (ID3D11Resource*) texture;

        GrabsLog.Log("Creating render target");
        fixed (ID3D11RenderTargetView** renderTarget = &RenderTarget)
            device->CreateRenderTargetView(_texture, null, renderTarget).Check("Create render target");
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        if (RenderTarget != null)
            RenderTarget->Release();

        _texture->Release();
    }
}