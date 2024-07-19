using System;
using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D11_FILTER;
using static grabs.Graphics.D3D11.D3DResult;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11Sampler : Sampler
{
    public ID3D11SamplerState* SamplerState;
    
    public D3D11Sampler(ID3D11Device* device, in SamplerDescription description) : base(description)
    {
        D3D11_FILTER filter;
        if (description.EnableAnisotropy)
            filter = D3D11_FILTER_ANISOTROPIC;
        else
        {
            filter = (description.MinFilter, description.MagFilter, description.MipFilter) switch
            {
                (TextureFilter.Point, TextureFilter.Point, TextureFilter.Point) => D3D11_FILTER_MIN_MAG_MIP_POINT,
                (TextureFilter.Point, TextureFilter.Point, TextureFilter.Linear) => D3D11_FILTER_MIN_MAG_POINT_MIP_LINEAR,
                (TextureFilter.Point, TextureFilter.Linear, TextureFilter.Point) => D3D11_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT,
                (TextureFilter.Point, TextureFilter.Linear, TextureFilter.Linear) => D3D11_FILTER_MIN_POINT_MAG_MIP_LINEAR,
                (TextureFilter.Linear, TextureFilter.Point, TextureFilter.Point) => D3D11_FILTER_MIN_LINEAR_MAG_MIP_POINT,
                (TextureFilter.Linear, TextureFilter.Point, TextureFilter.Linear) => D3D11_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR,
                (TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Point) => D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT,
                (TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Linear) => D3D11_FILTER_MIN_MAG_MIP_LINEAR,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        D3D11_SAMPLER_DESC desc = new()
        {
            Filter = filter,
            AddressU = D3D11Utils.TextureAddressToD3D(description.AddressU),
            AddressV = D3D11Utils.TextureAddressToD3D(description.AddressV),
            AddressW = D3D11Utils.TextureAddressToD3D(description.AddressW),
            MipLODBias = description.MipLodBias,
            MaxAnisotropy = description.MaxAnisotropy,
            ComparisonFunc = D3D11Utils.ComparisonFunctionToD3D(description.Comparison),
            MinLOD = description.MinLod,
            MaxLOD = description.MaxLod
        };

        // TODO: Is there a better way of doing this?
        desc.BorderColor[0] = description.BorderColor.X;
        desc.BorderColor[1] = description.BorderColor.Y;
        desc.BorderColor[2] = description.BorderColor.Z;
        desc.BorderColor[3] = description.BorderColor.W;
        
        fixed (ID3D11SamplerState** samplerState = &SamplerState)
            CheckResult(device->CreateSamplerState(&desc, samplerState), "Create sampler state");
    }
    
    public override void Dispose()
    {
        SamplerState->Release();
    }
}