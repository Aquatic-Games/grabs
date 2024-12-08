using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Texture : Texture
{
    public readonly ID3D11Resource* Texture;

    public readonly ID3D11ShaderResourceView* TextureSrv;

    public readonly ID3D11RenderTargetView* RenderTarget;

    public readonly ID3D11DepthStencilView* DepthTarget;

    public D3D11Texture(ID3D11Texture2D* texture, ID3D11RenderTargetView* target)
    {
        Texture = (ID3D11Resource*) texture;
        RenderTarget = target;
    }
    
    public override void Dispose()
    {
        if (DepthTarget != null)
            DepthTarget->Release();

        if (RenderTarget != null)
            RenderTarget->Release();

        if (TextureSrv != null)
            TextureSrv->Release();

        Texture->Release();
    }
}