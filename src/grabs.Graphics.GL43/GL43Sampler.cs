using System;
using System.Numerics;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Sampler : Sampler
{
    private readonly GL _gl;

    public readonly uint Sampler;

    public unsafe GL43Sampler(GL gl, in SamplerDescription description) : base(in description)
    {
        _gl = gl;
        
        Sampler = _gl.CreateSampler();

        TextureMinFilter minFilter = (description.MinFilter, description.MipFilter) switch
        {
            (TextureFilter.Point, TextureFilter.Point) => TextureMinFilter.NearestMipmapNearest,
            (TextureFilter.Point, TextureFilter.Linear) => TextureMinFilter.NearestMipmapLinear,
            (TextureFilter.Linear, TextureFilter.Point) => TextureMinFilter.LinearMipmapNearest,
            (TextureFilter.Linear, TextureFilter.Linear) => TextureMinFilter.LinearMipmapLinear,
            _ => throw new ArgumentOutOfRangeException()
        };

        TextureMagFilter magFilter = description.MagFilter switch
        {
            TextureFilter.Point => TextureMagFilter.Nearest,
            TextureFilter.Linear => TextureMagFilter.Linear,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _gl.SamplerParameter(Sampler, SamplerParameterI.MinFilter, (int) minFilter);
        _gl.SamplerParameter(Sampler, SamplerParameterI.MagFilter, (int) magFilter);
        
        _gl.SamplerParameter(Sampler, SamplerParameterI.WrapS, (int) GLUtils.TextureAddressToGL(description.AddressU));
        _gl.SamplerParameter(Sampler, SamplerParameterI.WrapT, (int) GLUtils.TextureAddressToGL(description.AddressV));
        _gl.SamplerParameter(Sampler, SamplerParameterI.WrapR, (int) GLUtils.TextureAddressToGL(description.AddressW));
        
        _gl.SamplerParameter(Sampler, SamplerParameterF.LodBias, description.MipLodBias);

        if (description.EnableAnisotropy)
            _gl.SamplerParameter(Sampler, SamplerParameterF.MaxAnisotropy, description.MaxAnisotropy);

        _gl.SamplerParameter(Sampler, SamplerParameterI.CompareFunc,
            (int) GLUtils.ComparisonFunctionToGL(description.Comparison));
        
        _gl.SamplerParameter(Sampler, SamplerParameterF.MinLod, description.MinLod);
        _gl.SamplerParameter(Sampler, SamplerParameterF.MaxLod, description.MaxLod);
        
        Vector4 borderColor = description.BorderColor;
        _gl.SamplerParameter(Sampler, SamplerParameterF.BorderColor, &borderColor.X);
    }
    
    public override void Dispose()
    {
        _gl.DeleteSampler(Sampler);
    }
}