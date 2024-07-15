using System;
using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using static TerraFX.Interop.DirectX.D3D11_RESOURCE_MISC_FLAG;
using static TerraFX.Interop.DirectX.D3D11_USAGE;
using static grabs.Graphics.D3D11.D3DResult;
using static TerraFX.Interop.DirectX.D3D_SRV_DIMENSION;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11Texture : Texture
{
    public readonly ID3D11Resource* Texture;
    public readonly ID3D11ShaderResourceView* ResourceView;

    public readonly Format Format;

    public D3D11Texture(ID3D11Device* device, ID3D11DeviceContext* context, in TextureDescription description,
        void** ppData) : base(description)
    {
        Format = description.Format;
        
        D3D11_BIND_FLAG flags = 0;

        if ((description.Usage & TextureUsage.ShaderResource) == TextureUsage.ShaderResource)
            flags |= D3D11_BIND_SHADER_RESOURCE;

        if ((description.Usage & TextureUsage.Framebuffer) == TextureUsage.Framebuffer || (description.Usage & TextureUsage.GenerateMips) == TextureUsage.GenerateMips)
            flags |= D3D11_BIND_RENDER_TARGET;

        if (description.Format is Format.D32_Float or Format.D16_UNorm or Format.D24_UNorm_S8_UInt)
            flags |= D3D11_BIND_DEPTH_STENCIL;

        uint mipLevels = description.MipLevels == 0
            ? GraphicsUtils.CalculateMipLevels(description.Width, description.Height)
            : description.MipLevels;
        
        D3D11_SHADER_RESOURCE_VIEW_DESC srvDesc = new()
        {
            Format = D3D11Utils.FormatToD3D(Format)
        };

        switch (description.Type)
        {
            case TextureType.Texture2D:
            {
                D3D11_TEXTURE2D_DESC desc = new()
                {
                    Width = description.Width,
                    Height = description.Height,
                    Format = srvDesc.Format,
                    ArraySize = 1,
                    MipLevels = mipLevels,
                    SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
                    Usage = D3D11_USAGE_DEFAULT,
                    BindFlags = (uint) flags,
                    CPUAccessFlags = 0,
                    MiscFlags = (description.Usage & TextureUsage.GenerateMips) == TextureUsage.GenerateMips
                        ? (uint) D3D11_RESOURCE_MISC_GENERATE_MIPS
                        : 0
                };

                fixed (ID3D11Resource** texture = &Texture)
                    CheckResult(device->CreateTexture2D(&desc, null, (ID3D11Texture2D**) texture), "Create Texture2D");

                srvDesc.ViewDimension = D3D_SRV_DIMENSION_TEXTURE2D;
                srvDesc.Texture2D = new D3D11_TEX2D_SRV()
                {
                    MipLevels = uint.MaxValue,
                    MostDetailedMip = 0
                };
                
                break;
            }

            case TextureType.Cubemap:
            {
                D3D11_TEXTURE2D_DESC desc = new()
                {
                    Width = description.Width,
                    Height = description.Height,
                    Format = srvDesc.Format,
                    ArraySize = 6,
                    MipLevels = mipLevels,
                    SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
                    Usage = D3D11_USAGE_DEFAULT,
                    BindFlags = (uint) flags,
                    MiscFlags = (uint) D3D11_RESOURCE_MISC_TEXTURECUBE |
                                ((description.Usage & TextureUsage.GenerateMips) == TextureUsage.GenerateMips
                                    ? (uint) D3D11_RESOURCE_MISC_GENERATE_MIPS
                                    : 0)
                };

                fixed (ID3D11Resource** texture = &Texture)
                    CheckResult(device->CreateTexture2D(&desc, null, (ID3D11Texture2D**) texture), "Create Cubemap");

                srvDesc.ViewDimension = D3D_SRV_DIMENSION_TEXTURECUBE;
                srvDesc.TextureCube = new D3D11_TEXCUBE_SRV()
                {
                    MipLevels = uint.MaxValue,
                    MostDetailedMip = 0
                };

                break;
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (ppData != null)
        {
            uint pitch = GraphicsUtils.CalculatePitch(description.Format, description.Width);

            // TODO: This is a terrible implementation. It doesn't handle arrays or mipmaps or anything. Just cubemaps.
            for (uint a = 0; a < (description.Type == TextureType.Cubemap ? 6 : 1); a++)
            {
                context->UpdateSubresource(Texture, D3D11Utils.CalculateSubresource(0, a, mipLevels), null, ppData[a],
                    pitch, 0);
            }
        }

        // TODO: TextureView in GRABS
        if ((description.Usage & TextureUsage.ShaderResource) != 0)
        {
            fixed (ID3D11ShaderResourceView** resourceView = &ResourceView)
                CheckResult(device->CreateShaderResourceView(Texture, &srvDesc, resourceView), "Create SRV");
        }
    }

    public D3D11Texture(ID3D11Resource* texture, ID3D11ShaderResourceView* resourceView) : base(new TextureDescription())
    {
        Texture = texture;
        ResourceView = resourceView;
    }
    
    public override void Dispose()
    {
        if (ResourceView != null)
            ResourceView->Release();

        Texture->Release();
    }
}