using grabs.Core;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Texture : Texture
{
    public readonly ID3D11Resource Texture;

    public readonly ID3D11ShaderResourceView? ResourceView;

    public readonly ID3D11RenderTargetView? RenderTarget;
    
    public override Size2D Size { get; }

    public unsafe D3D11Texture(ID3D11Device device, ref readonly TextureInfo info, void* pData)
    {
        BindFlags bindFlags = BindFlags.None;
        if (info.Usage.HasFlag(TextureUsage.Sampled))
            bindFlags |= BindFlags.ShaderResource;

        ShaderResourceViewDescription viewDesc = new()
        {
            Format = info.Format.ToD3D()
        };
        
        switch (info.Type)
        {
            case TextureType.Texture2D:
            {
                Texture2DDescription desc = new()
                {
                    Width = info.Size.Width,
                    Height = info.Size.Height,
                    Format = viewDesc.Format,
                    ArraySize = 1,
                    MipLevels = 1,
                    Usage = ResourceUsage.Default,
                    BindFlags = bindFlags,
                    SampleDescription = new SampleDescription(1, 0),
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = ResourceOptionFlags.None
                };

                SubresourceData subData = new SubresourceData(pData);
                Texture = device.CreateTexture2D(in desc, subData);

                viewDesc.ViewDimension = ShaderResourceViewDimension.Texture2D;
                viewDesc.Texture2D = new Texture2DShaderResourceView()
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                };

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (info.Usage.HasFlag(TextureUsage.Sampled))
            ResourceView = device.CreateShaderResourceView(Texture, viewDesc);
    }

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