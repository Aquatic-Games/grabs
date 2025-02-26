global using DxgiFormat = Vortice.DXGI.Format;
using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal static class D3D11Utils
{
    public static DxgiFormat ToD3D(this Format format)
    {
        return format switch
        {
            Format.B5G6R5_UNorm => DxgiFormat.B5G6R5_UNorm,
            Format.B5G5R5A1_UNorm => DxgiFormat.B5G5R5A1_UNorm,
            Format.R8_UNorm => DxgiFormat.R8_UNorm,
            Format.R8_UInt => DxgiFormat.R8_UInt,
            Format.R8_SNorm => DxgiFormat.R8_SNorm,
            Format.R8_SInt => DxgiFormat.R8_SInt,
            Format.A8_UNorm => DxgiFormat.A8_UNorm,
            Format.R8G8_UNorm => DxgiFormat.R8G8_UNorm,
            Format.R8G8_UInt => DxgiFormat.R8G8_UInt,
            Format.R8G8_SNorm => DxgiFormat.R8G8_SNorm,
            Format.R8G8_SInt => DxgiFormat.R8G8_SInt,
            Format.R8G8B8A8_UNorm => DxgiFormat.R8G8B8A8_UNorm,
            Format.R8G8B8A8_UNorm_SRGB => DxgiFormat.R8G8B8A8_UNorm_SRgb,
            Format.R8G8B8A8_UInt => DxgiFormat.R8G8B8A8_UInt,
            Format.R8G8B8A8_SNorm => DxgiFormat.R8G8B8A8_SNorm,
            Format.R8G8B8A8_SInt => DxgiFormat.R8G8B8A8_SInt,
            Format.B8G8R8A8_UNorm => DxgiFormat.B8G8R8A8_UNorm,
            Format.B8G8R8A8_UNorm_SRGB => DxgiFormat.B8G8R8A8_UNorm_SRgb,
            Format.R16_Float => DxgiFormat.R16_Float,
            Format.D16_UNorm => DxgiFormat.D16_UNorm,
            Format.R16_UNorm => DxgiFormat.R16_UNorm,
            Format.R16_UInt => DxgiFormat.R16_UInt,
            Format.R16_SNorm => DxgiFormat.R16_SNorm,
            Format.R16_SInt => DxgiFormat.R16_SInt,
            Format.R16G16_Float => DxgiFormat.R16G16_Float,
            Format.R16G16_UNorm => DxgiFormat.R16G16_UNorm,
            Format.R16G16_UInt => DxgiFormat.R16G16_UInt,
            Format.R16G16_SNorm => DxgiFormat.R16G16_SNorm,
            Format.R16G16_SInt => DxgiFormat.R16G16_SInt,
            Format.R16G16B16A16_Float => DxgiFormat.R16G16B16A16_Float,
            Format.R16G16B16A16_UNorm => DxgiFormat.R16G16B16A16_UNorm,
            Format.R16G16B16A16_UInt => DxgiFormat.R16G16B16A16_UInt,
            Format.R16G16B16A16_SNorm => DxgiFormat.R16G16B16A16_SNorm,
            Format.R16G16B16A16_SInt => DxgiFormat.R16G16B16A16_SInt,
            Format.R32_Float => DxgiFormat.R32_Float,
            Format.R32_UInt => DxgiFormat.R32_UInt,
            Format.R32_SInt => DxgiFormat.R32_SInt,
            Format.R32G32_Float => DxgiFormat.R32G32_Float,
            Format.R32G32_UInt => DxgiFormat.R32G32_UInt,
            Format.R32G32_SInt => DxgiFormat.R32G32_SInt,
            Format.R32G32B32_Float => DxgiFormat.R32G32B32_Float,
            Format.R32G32B32_UInt => DxgiFormat.R32G32B32_UInt,
            Format.R32G32B32_SInt => DxgiFormat.R32G32B32_SInt,
            Format.R32G32B32A32_Float => DxgiFormat.R32G32B32A32_Float,
            Format.R32G32B32A32_UInt => DxgiFormat.R32G32B32A32_UInt,
            Format.R32G32B32A32_SInt => DxgiFormat.R32G32B32A32_SInt,
            Format.D24_UNorm_S8_UInt => DxgiFormat.D24_UNorm_S8_UInt,
            Format.D32_Float => DxgiFormat.D32_Float,
            Format.BC1_UNorm => DxgiFormat.BC1_UNorm,
            Format.BC1_UNorm_SRGB => DxgiFormat.BC1_UNorm_SRgb,
            Format.BC2_UNorm => DxgiFormat.BC2_UNorm,
            Format.BC2_UNorm_SRGB => DxgiFormat.BC2_UNorm_SRgb,
            Format.BC3_UNorm => DxgiFormat.BC3_UNorm,
            Format.BC3_UNorm_SRGB => DxgiFormat.BC3_UNorm_SRgb,
            Format.BC4_UNorm => DxgiFormat.BC4_UNorm,
            Format.BC4_SNorm => DxgiFormat.BC4_SNorm,
            Format.BC5_UNorm => DxgiFormat.BC5_UNorm,
            Format.BC5_SNorm => DxgiFormat.BC5_SNorm,
            Format.BC6H_UF16 => DxgiFormat.BC6H_Uf16,
            Format.BC6H_SF16 => DxgiFormat.BC6H_Sf16,
            Format.BC7_UNorm => DxgiFormat.BC7_UNorm,
            Format.BC7_UNorm_SRGB => DxgiFormat.BC7_UNorm_SRgb,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static Vortice.Direct3D11.MapMode ToD3D(this MapMode mode)
    {
        return mode switch
        {
            MapMode.Write => Vortice.Direct3D11.MapMode.WriteDiscard,
            MapMode.Read => Vortice.Direct3D11.MapMode.Read,
            MapMode.ReadAndWrite => Vortice.Direct3D11.MapMode.ReadWrite,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}