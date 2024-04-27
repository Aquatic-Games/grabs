using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Texture : Texture
{
    public readonly ID3D11Resource Texture;
    public readonly ID3D11ShaderResourceView ResourceView;

    public unsafe D3D11Texture(ID3D11Device device, ID3D11DeviceContext context, in TextureDescription description,
        void* pData)
    {
        BindFlags flags = BindFlags.None;

        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
            flags |= BindFlags.ShaderResource;

        if ((description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer || (description.Usage & TextureUsage.GenerateMips) == TextureUsage.GenerateMips)
            flags |= BindFlags.RenderTarget;

        if (description.Format is Format.D32_Float or Format.D16_UNorm or Format.D24_UNorm_S8_UInt)
            flags |= BindFlags.DepthStencil;
        
        ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription()
        {
            Format = description.Format.ToDXGIFormat()
        };

        switch (description.Type)
        {
            case TextureType.Texture2D:
            {
                Texture2DDescription desc = new Texture2DDescription()
                {
                    Width = (int) description.Width,
                    Height = (int) description.Height,
                    Format = srvDesc.Format,
                    ArraySize = 1,
                    MipLevels = (int) description.MipLevels,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = flags,
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = (description.Usage & TextureUsage.GenerateMips) == TextureUsage.GenerateMips
                        ? ResourceOptionFlags.GenerateMips
                        : ResourceOptionFlags.None
                };

                Texture = device.CreateTexture2D(desc);

                srvDesc.ViewDimension = ShaderResourceViewDimension.Texture2D;
                srvDesc.Texture2D = new Texture2DShaderResourceView()
                {
                    MipLevels = -1,
                    MostDetailedMip = 0
                };
                
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (pData != null)
        {
            uint pitch = GrabsUtils.CalculatePitch(description.Format, description.Width);
            context.UpdateSubresource(Texture, 0, null, (nint) pData, (int) pitch, 0);
        }

        // TODO: TextureView in GRABS
        if ((description.Usage & TextureUsage.ShaderResource) != 0)
            ResourceView = device.CreateShaderResourceView(Texture, srvDesc);
    }

    public D3D11Texture(ID3D11Resource texture, ID3D11ShaderResourceView resourceView)
    {
        Texture = texture;
        ResourceView = resourceView;
    }
    
    public override void Dispose()
    {
        ResourceView?.Dispose();
        
        Texture.Dispose();
    }
}