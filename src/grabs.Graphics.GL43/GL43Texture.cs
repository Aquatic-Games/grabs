﻿using System;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Texture : Texture
{
    private readonly GL _gl;

    private readonly SizedInternalFormat _internalFormat;
    private readonly PixelFormat _format;
    private readonly PixelType _pixelType;

    public readonly uint Texture;
    public readonly TextureTarget Target;

    public readonly bool IsRenderbuffer;

    public unsafe GL43Texture(GL gl, in TextureDescription description, void** ppData) : base(description)
    {
        _gl = gl;

        Target = description.Type switch
        {
            TextureType.Texture2D => TextureTarget.Texture2D,
            TextureType.Cubemap => TextureTarget.TextureCubeMap,
            _ => throw new ArgumentOutOfRangeException()
        };

        Format format = description.Format;

        // i think this legitimately gave me a brain aneurysm
        // TODO: Test these
        (_internalFormat, _format, _pixelType) = format switch
        {
            Format.R8G8B8A8_UNorm => (SizedInternalFormat.Rgba8, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.B5G6R5_UNorm => (SizedInternalFormat.Rgb565, PixelFormat.Bgr, PixelType.UnsignedShort565),
            Format.B5G5R5A1_UNorm => (SizedInternalFormat.Rgba8, PixelFormat.Bgra, PixelType.UnsignedShort5551),
            Format.R8_UNorm => (SizedInternalFormat.R8, PixelFormat.Red, PixelType.UnsignedByte),
            Format.R8_UInt => (SizedInternalFormat.R8i, PixelFormat.Red, PixelType.UnsignedInt),
            Format.R8_SNorm => (SizedInternalFormat.R8SNorm, PixelFormat.Red, PixelType.Byte),
            Format.R8_SInt => (SizedInternalFormat.R8i, PixelFormat.Red, PixelType.Int),
            Format.A8_UNorm => (SizedInternalFormat.Alpha8Ext, PixelFormat.Alpha, PixelType.UnsignedByte),
            Format.R8G8_UNorm => (SizedInternalFormat.RG8, PixelFormat.RG, PixelType.UnsignedByte),
            Format.R8G8_UInt => (SizedInternalFormat.RG8, PixelFormat.RG, PixelType.UnsignedInt),
            Format.R8G8_SNorm => (SizedInternalFormat.RG8, PixelFormat.RG, PixelType.Byte),
            Format.R8G8_SInt => (SizedInternalFormat.RG8, PixelFormat.RG, PixelType.Int),
            Format.R8G8B8A8_UNorm_SRGB => (SizedInternalFormat.Srgb8Alpha8, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.R8G8B8A8_UInt => (SizedInternalFormat.Rgba8i, PixelFormat.Rgba, PixelType.UnsignedInt),
            Format.R8G8B8A8_SNorm => (SizedInternalFormat.Rgba8, PixelFormat.Rgba, PixelType.Byte),
            Format.R8G8B8A8_SInt => (SizedInternalFormat.Rgba8i, PixelFormat.Rgba, PixelType.Int),
            Format.B8G8R8A8_UNorm => (SizedInternalFormat.Rgba8, PixelFormat.Bgra, PixelType.UnsignedByte),
            Format.B8G8R8A8_UNorm_SRGB => (SizedInternalFormat.Srgb8Alpha8, PixelFormat.Bgra, PixelType.UnsignedByte),
            Format.R10G10B10A2_UNorm => (SizedInternalFormat.Rgb10A2, PixelFormat.Rgba, PixelType.UnsignedInt1010102),
            Format.R10G10B10A2_UInt => (SizedInternalFormat.Rgb10A2ui, PixelFormat.Rgba, PixelType.UnsignedInt1010102),
            Format.R11G11B10_Float => (SizedInternalFormat.R11fG11fB10f, PixelFormat.Rgb, PixelType.Float),
            Format.R16_Float => (SizedInternalFormat.R16f, PixelFormat.Red, PixelType.Float),
            Format.D16_UNorm => (SizedInternalFormat.DepthComponent16, PixelFormat.DepthComponent, PixelType.UnsignedShort),
            Format.R16_UNorm => (SizedInternalFormat.R16, PixelFormat.Red, PixelType.UnsignedShort),
            Format.R16_UInt => (SizedInternalFormat.R16ui, PixelFormat.Red, PixelType.UnsignedInt),
            Format.R16_SNorm => (SizedInternalFormat.R16SNorm, PixelFormat.Red, PixelType.Short),
            Format.R16_SInt => (SizedInternalFormat.R16i, PixelFormat.Red, PixelType.Int),
            Format.R16G16_Float => (SizedInternalFormat.RG16f, PixelFormat.RG, PixelType.Float),
            Format.R16G16_UNorm => (SizedInternalFormat.RG16, PixelFormat.RG, PixelType.UnsignedShort),
            Format.R16G16_UInt => (SizedInternalFormat.RG16ui, PixelFormat.RG, PixelType.UnsignedInt),
            Format.R16G16_SNorm => (SizedInternalFormat.RG16SNorm, PixelFormat.RG, PixelType.Short),
            Format.R16G16_SInt => (SizedInternalFormat.RG16i, PixelFormat.RG, PixelType.Int),
            Format.R16G16B16A16_Float => (SizedInternalFormat.Rgb16f, PixelFormat.Rgba, PixelType.Float),
            Format.R16G16B16A16_UNorm => (SizedInternalFormat.Rgba16, PixelFormat.Rgba, PixelType.UnsignedShort),
            Format.R16G16B16A16_UInt => (SizedInternalFormat.Rgba16ui, PixelFormat.Rgba, PixelType.UnsignedInt),
            Format.R16G16B16A16_SNorm => (SizedInternalFormat.Rgba16SNorm, PixelFormat.Rgba, PixelType.Short),
            Format.R16G16B16A16_SInt => (SizedInternalFormat.Rgba16i, PixelFormat.Rgba, PixelType.Int),
            Format.R32_Float => (SizedInternalFormat.R32f, PixelFormat.Red, PixelType.Float),
            Format.R32_UInt => (SizedInternalFormat.R32ui, PixelFormat.Red, PixelType.UnsignedInt),
            Format.R32_SInt => (SizedInternalFormat.R32i, PixelFormat.Red, PixelType.Int),
            Format.R32G32_Float => (SizedInternalFormat.RG32f, PixelFormat.RG, PixelType.Float),
            Format.R32G32_UInt => (SizedInternalFormat.RG32ui, PixelFormat.RG, PixelType.UnsignedInt),
            Format.R32G32_SInt => (SizedInternalFormat.RG32i, PixelFormat.RG, PixelType.Int),
            Format.R32G32B32_Float => (SizedInternalFormat.Rgb32f, PixelFormat.Rgb, PixelType.Float),
            Format.R32G32B32_UInt => (SizedInternalFormat.Rgb32ui, PixelFormat.Rgb, PixelType.UnsignedInt),
            Format.R32G32B32_SInt => (SizedInternalFormat.Rgb32i, PixelFormat.Rgb, PixelType.Int),
            Format.R32G32B32A32_Float => (SizedInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float),
            Format.R32G32B32A32_UInt => (SizedInternalFormat.Rgba32ui, PixelFormat.Rgba, PixelType.UnsignedInt),
            Format.R32G32B32A32_SInt => (SizedInternalFormat.Rgba32i, PixelFormat.Rgba, PixelType.Int),
            Format.D24_UNorm_S8_UInt => (SizedInternalFormat.Depth24Stencil8, PixelFormat.DepthStencil, PixelType.UnsignedByte), // ????
            Format.D32_Float => (SizedInternalFormat.DepthComponent32f, PixelFormat.DepthComponent, PixelType.Float),
            Format.BC1_UNorm => (SizedInternalFormat.CompressedRgbaS3TCDxt1Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.BC1_UNorm_SRGB => (SizedInternalFormat.CompressedSrgbAlphaS3TCDxt1Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.BC2_UNorm => (SizedInternalFormat.CompressedRgbaS3TCDxt3Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.BC2_UNorm_SRGB => (SizedInternalFormat.CompressedSrgbAlphaS3TCDxt3Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.BC3_UNorm => (SizedInternalFormat.CompressedRgbaS3TCDxt5Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.BC3_UNorm_SRGB => (SizedInternalFormat.CompressedSrgbAlphaS3TCDxt5Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.BC4_UNorm => (SizedInternalFormat.CompressedRedRgtc1, PixelFormat.Red, PixelType.UnsignedByte),
            Format.BC4_SNorm => (SizedInternalFormat.CompressedSignedRedRgtc1, PixelFormat.Red, PixelType.UnsignedByte),
            Format.BC5_UNorm => (SizedInternalFormat.CompressedRGRgtc2, PixelFormat.RG, PixelType.UnsignedByte),
            Format.BC5_SNorm => (SizedInternalFormat.CompressedSignedRGRgtc2, PixelFormat.RG, PixelType.UnsignedByte),
            Format.BC6H_UF16 => (SizedInternalFormat.CompressedRgbBptcUnsignedFloat, PixelFormat.Rgb, PixelType.UnsignedByte),
            Format.BC6H_SF16 => (SizedInternalFormat.CompressedRgbBptcSignedFloat, PixelFormat.Rgb, PixelType.UnsignedByte),
            Format.BC7_UNorm => (SizedInternalFormat.CompressedRgbaBptcUnorm, PixelFormat.Rgba, PixelType.UnsignedByte),
            Format.BC7_UNorm_SRGB => (SizedInternalFormat.CompressedSrgbAlphaBptcUnorm, PixelFormat.Rgba, PixelType.UnsignedByte),
            _ => throw new NotImplementedException()
        };

        // Renderbuffer requirements:
        //   - Must not be a shader resource
        //   - Must be a Texture2D
        //   - Must not have initial data
        //   - Must only have 1 mip level
        // If these requirements are met, then it will be created as a renderbuffer instead.
        // If this causes issues later down the line then it will be re-evaluated.
        if ((description.Usage & TextureUsage.ShaderResource) == 0 && description.Type == TextureType.Texture2D && ppData == null && description.MipLevels == 1)
        {
            Console.WriteLine("Info: Texture creating as renderbuffer.");
            IsRenderbuffer = true;
            Texture = _gl.GenRenderbuffer();
            _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, Texture);
            _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (InternalFormat) _internalFormat, description.Width,
                description.Height);

            return;
        }
        
        Texture = _gl.GenTexture();
        Console.WriteLine($"ID: {Texture}, Size: {description.Width}x{description.Height}");
        _gl.BindTexture(Target, Texture);

        bool isCompressed = format.IsCompressed();
        
        // If mip levels are 0 then calculate the number of mip levels.
        uint mipLevels = description.MipLevels == 0
            ? (uint) float.Floor(float.Log2(uint.Max(description.Width, description.Height))) + 1
            : description.MipLevels;

        uint width = description.Width;
        uint height = description.Height;

        switch (description.Type)
        {
            case TextureType.Texture2D:
                _gl.TexStorage2D(Target, mipLevels, _internalFormat, width, height);

                if (ppData != null)
                {
                    if (isCompressed)
                    {
                        _gl.CompressedTexSubImage2D(Target, 0, 0, 0, width, height, (InternalFormat) _internalFormat,
                            GraphicsUtils.CalculateTextureSizeInBytes(format, width, height), ppData[0]);
                    }
                    else
                    {
                        _gl.TexSubImage2D(Target, 0, 0, 0, description.Width, description.Height, _format, _pixelType,
                            ppData[0]);
                    }
                }

                break;

            case TextureType.Cubemap:
            {
                _gl.TexStorage2D(TextureTarget.TextureCubeMap, mipLevels, _internalFormat, width, height);

                if (ppData != null)
                {
                    for (int a = 0; a < 6; a++)
                    {
                        TextureTarget target = TextureTarget.TextureCubeMapPositiveX + a;
                        void* pData = ppData[a];

                        if (isCompressed)
                        {
                            _gl.CompressedTexSubImage2D(target, 0, 0, 0, width, height,
                                (InternalFormat) _internalFormat,
                                GraphicsUtils.CalculateTextureSizeInBytes(format, width, height), pData);
                        }
                        else
                        {
                            _gl.TexSubImage2D(target, 0, 0, 0, description.Width, description.Height, _format,
                                _pixelType, pData);
                        }
                    }
                }

                break;
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public unsafe void Update(int x, int y, uint width, uint height, uint mipLevel, void* pData)
    {
        _gl.BindTexture(Target, Texture);

        bool isCompressed = Description.Format.IsCompressed();
        
        switch (Description.Type)
        {
            case TextureType.Texture2D:
            {
                if (isCompressed)
                {
                    _gl.CompressedTexSubImage2D(Target, (int) mipLevel, x, y, width, height,
                        (InternalFormat) _internalFormat,
                        GraphicsUtils.CalculateTextureSizeInBytes(Description.Format, width, height), pData);
                }
                else
                    _gl.TexSubImage2D(Target, (int) mipLevel, x, y, width, height, _format, _pixelType, pData);
                
                break;
            }
            case TextureType.Cubemap:
                throw new NotImplementedException("Cannot update cubemap texture currently.");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Dispose()
    {
        if (IsRenderbuffer)
            _gl.DeleteRenderbuffer(Texture);
        else
            _gl.DeleteTexture(Texture);
    }
}