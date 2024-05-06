﻿using System;
using DXGIFormat = Vortice.DXGI.Format;

namespace grabs.Graphics.D3D11;

public static class D3D11Utils
{
    public static DXGIFormat ToDXGIFormat(this Format format)
    {
        return format switch
        {
            Format.B5G6R5_UNorm => DXGIFormat.B5G6R5_UNorm,
            Format.B5G5R5A1_UNorm => DXGIFormat.B5G5R5A1_UNorm,
            Format.R8_UNorm => DXGIFormat.R8_UNorm,
            Format.R8_UInt => DXGIFormat.R8_UInt,
            Format.R8_SNorm => DXGIFormat.R8_SNorm,
            Format.R8_SInt => DXGIFormat.R8_SInt,
            Format.A8_UNorm => DXGIFormat.A8_UNorm,
            Format.R8G8_UNorm => DXGIFormat.R8G8_UNorm,
            Format.R8G8_UInt => DXGIFormat.R8G8_UInt,
            Format.R8G8_SNorm => DXGIFormat.R8G8_SNorm,
            Format.R8G8_SInt => DXGIFormat.R8G8_SInt,
            Format.R8G8B8A8_UNorm => DXGIFormat.R8G8B8A8_UNorm,
            Format.R8G8B8A8_UNorm_SRGB => DXGIFormat.R8G8B8A8_UNorm_SRgb,
            Format.R8G8B8A8_UInt => DXGIFormat.R8G8B8A8_UInt,
            Format.R8G8B8A8_SNorm => DXGIFormat.R8G8B8A8_SNorm,
            Format.R8G8B8A8_SInt => DXGIFormat.R8G8B8A8_SInt,
            Format.B8G8R8A8_UNorm => DXGIFormat.B8G8R8A8_UNorm,
            Format.B8G8R8A8_UNorm_SRGB => DXGIFormat.B8G8R8A8_UNorm_SRgb,
            Format.R10G10B10A2_UNorm => DXGIFormat.R10G10B10A2_UNorm,
            Format.R10G10B10A2_UInt => DXGIFormat.R10G10B10A2_UInt,
            Format.R11G11B10_Float => DXGIFormat.R11G11B10_Float,
            Format.R16_Float => DXGIFormat.R16_Float,
            Format.D16_UNorm => DXGIFormat.D16_UNorm,
            Format.R16_UNorm => DXGIFormat.R16_UNorm,
            Format.R16_UInt => DXGIFormat.R16_UInt,
            Format.R16_SNorm => DXGIFormat.R16_SNorm,
            Format.R16_SInt => DXGIFormat.R16_SInt,
            Format.R16G16_Float => DXGIFormat.R16G16_Float,
            Format.R16G16_UNorm => DXGIFormat.R16G16_UNorm,
            Format.R16G16_UInt => DXGIFormat.R16G16_UInt,
            Format.R16G16_SNorm => DXGIFormat.R16G16_SNorm,
            Format.R16G16_SInt => DXGIFormat.R16G16_SInt,
            Format.R16G16B16A16_Float => DXGIFormat.R16G16B16A16_Float,
            Format.R16G16B16A16_UNorm => DXGIFormat.R16G16B16A16_UNorm,
            Format.R16G16B16A16_UInt => DXGIFormat.R16G16B16A16_UInt,
            Format.R16G16B16A16_SNorm => DXGIFormat.R16G16B16A16_SNorm,
            Format.R16G16B16A16_SInt => DXGIFormat.R16G16B16A16_SInt,
            Format.R32_Float => DXGIFormat.R32_Float,
            Format.R32_UInt => DXGIFormat.R32_UInt,
            Format.R32_SInt => DXGIFormat.R32_SInt,
            Format.R32G32_Float => DXGIFormat.R32G32_Float,
            Format.R32G32_UInt => DXGIFormat.R32G32_UInt,
            Format.R32G32_SInt => DXGIFormat.R32G32_SInt,
            Format.R32G32B32_Float => DXGIFormat.R32G32B32_Float,
            Format.R32G32B32_UInt => DXGIFormat.R32G32B32_UInt,
            Format.R32G32B32_SInt => DXGIFormat.R32G32B32_SInt,
            Format.R32G32B32A32_Float => DXGIFormat.R32G32B32A32_Float,
            Format.R32G32B32A32_UInt => DXGIFormat.R32G32B32A32_UInt,
            Format.R32G32B32A32_SInt => DXGIFormat.R32G32B32A32_SInt,
            Format.D24_UNorm_S8_UInt => DXGIFormat.D24_UNorm_S8_UInt,
            Format.D32_Float => DXGIFormat.D32_Float,
            Format.BC1_UNorm => DXGIFormat.BC1_UNorm,
            Format.BC1_UNorm_SRGB => DXGIFormat.BC1_UNorm_SRgb,
            Format.BC2_UNorm => DXGIFormat.BC2_UNorm,
            Format.BC2_UNorm_SRGB => DXGIFormat.BC2_UNorm_SRgb,
            Format.BC3_UNorm => DXGIFormat.BC3_UNorm,
            Format.BC3_UNorm_SRGB => DXGIFormat.BC3_UNorm_SRgb,
            Format.BC4_UNorm => DXGIFormat.BC4_UNorm,
            Format.BC4_SNorm => DXGIFormat.BC4_SNorm,
            Format.BC5_UNorm => DXGIFormat.BC5_UNorm,
            Format.BC5_SNorm => DXGIFormat.BC5_SNorm,
            Format.BC6H_UF16 => DXGIFormat.BC6H_Uf16,
            Format.BC6H_SF16 => DXGIFormat.BC6H_Sf16,
            Format.BC7_UNorm => DXGIFormat.BC7_UNorm,
            Format.BC7_UNorm_SRGB => DXGIFormat.BC7_UNorm_SRgb,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static Vortice.Direct3D11.ComparisonFunction ToComparisonFunc(this ComparisonFunction func)
    {
        return func switch
        {
            ComparisonFunction.Never => Vortice.Direct3D11.ComparisonFunction.Never,
            ComparisonFunction.Less => Vortice.Direct3D11.ComparisonFunction.Less,
            ComparisonFunction.Equal => Vortice.Direct3D11.ComparisonFunction.Equal,
            ComparisonFunction.LessEqual => Vortice.Direct3D11.ComparisonFunction.LessEqual,
            ComparisonFunction.Greater => Vortice.Direct3D11.ComparisonFunction.Greater,
            ComparisonFunction.NotEqual => Vortice.Direct3D11.ComparisonFunction.NotEqual,
            ComparisonFunction.GreaterEqual => Vortice.Direct3D11.ComparisonFunction.GreaterEqual,
            ComparisonFunction.Always => Vortice.Direct3D11.ComparisonFunction.Always,
            _ => throw new ArgumentOutOfRangeException(nameof(func), func, null)
        };
    }
}