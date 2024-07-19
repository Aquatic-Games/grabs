using System.Numerics;

namespace grabs.Graphics;

public struct SamplerDescription
{
    public TextureFilter MinFilter;
    
    public TextureFilter MagFilter;
    
    public TextureFilter MipFilter;

    public TextureAddress AddressU;

    public TextureAddress AddressV;

    public TextureAddress AddressW;

    public float MipLodBias;

    public bool EnableAnisotropy;

    public uint MaxAnisotropy;

    public ComparisonFunction Comparison;

    public float MinLod;

    public float MaxLod;

    public Vector4 BorderColor;

    public SamplerDescription(TextureFilter minFilter, TextureFilter magFilter, TextureFilter mipFilter,
        TextureAddress addressU, TextureAddress addressV, TextureAddress addressW, float mipLodBias,
        bool enableAnisotropy, uint maxAnisotropy, ComparisonFunction comparison, float minLod, float maxLod,
        Vector4 borderColor)
    {
        MinFilter = minFilter;
        MagFilter = magFilter;
        MipFilter = mipFilter;
        AddressU = addressU;
        AddressV = addressV;
        AddressW = addressW;
        MipLodBias = mipLodBias;
        EnableAnisotropy = enableAnisotropy;
        MaxAnisotropy = maxAnisotropy;
        Comparison = comparison;
        MinLod = minLod;
        MaxLod = maxLod;
        BorderColor = borderColor;
    }

    public SamplerDescription(TextureFilter minFilter, TextureFilter magFilter, TextureFilter mipFilter,
        TextureAddress address, bool enableAnisotropy = false, uint maxAnisotropy = 0) : this(minFilter, magFilter,
        mipFilter, address, address, address, 0, enableAnisotropy, maxAnisotropy, ComparisonFunction.LessEqual,
        float.MinValue, float.MaxValue, Vector4.One) { }

    public SamplerDescription(TextureFilter filter, TextureAddress address, bool enableAnisotropy = false,
        uint maxAnisotropy = 0) : this(filter, filter, filter, address, enableAnisotropy, maxAnisotropy) { }

    public static SamplerDescription PointWrap =>
        new SamplerDescription(TextureFilter.Point, TextureAddress.RepeatWrap);

    public static SamplerDescription PointClamp =>
        new SamplerDescription(TextureFilter.Point, TextureAddress.ClampToEdge);

    public static SamplerDescription LinearWrap =>
        new SamplerDescription(TextureFilter.Linear, TextureAddress.RepeatWrap);

    public static SamplerDescription LinearClamp =>
        new SamplerDescription(TextureFilter.Linear, TextureAddress.ClampToEdge);

    public static SamplerDescription Anisotropic2x =>
        new SamplerDescription(TextureFilter.Linear, TextureAddress.RepeatWrap, true, 2);
    
    public static SamplerDescription Anisotropic4x =>
        new SamplerDescription(TextureFilter.Linear, TextureAddress.RepeatWrap, true, 4);
    
    public static SamplerDescription Anisotropic8x =>
        new SamplerDescription(TextureFilter.Linear, TextureAddress.RepeatWrap, true, 8);
    
    public static SamplerDescription Anisotropic16x =>
        new SamplerDescription(TextureFilter.Linear, TextureAddress.RepeatWrap, true, 16);
}