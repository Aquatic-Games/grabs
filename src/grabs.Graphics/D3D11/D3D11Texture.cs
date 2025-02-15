using grabs.Core;
using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Texture : Texture
{
    public readonly ID3D11DeviceChild Texture;

    public readonly ID3D11ShaderResourceView? ResourceView;

    public readonly ID3D11RenderTargetView? RenderTarget;
    
    public override Size2D Size { get; }

    public D3D11Texture(ID3D11Texture2D texture, ID3D11RenderTargetView target, Size2D size)
    {
        Texture = texture;
        RenderTarget = target;
        Size = size;
    }
    
    public override void Dispose()
    {
        RenderTarget?.Dispose();
        ResourceView?.Dispose();
        Texture.Dispose();
    }
}