using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Texture : Texture
{
    public readonly ID3D11Resource Texture;

    public D3D11Texture(ID3D11Resource texture)
    {
        Texture = texture;
    }
    
    public override void Dispose()
    {
        Texture.Dispose();
    }
}