using System;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

public static class VkUtils
{
    public static Silk.NET.Vulkan.Format FormatToVk(Format format)
    {
        return format switch
        {
            Format.B5G6R5_UNorm => Silk.NET.Vulkan.Format.B5G6R5UnormPack16,
            Format.B5G5R5A1_UNorm => Silk.NET.Vulkan.Format.B5G5R5A1UnormPack16,
            Format.R8_UNorm => Silk.NET.Vulkan.Format.R8Unorm,
            Format.R8_UInt => Silk.NET.Vulkan.Format.R8Uint,
            Format.R8_SNorm => Silk.NET.Vulkan.Format.R8SNorm,
            Format.R8_SInt => Silk.NET.Vulkan.Format.R8Sint,
            Format.A8_UNorm => Silk.NET.Vulkan.Format.A8UnormKhr,
            Format.R8G8_UNorm => Silk.NET.Vulkan.Format.R8G8Unorm,
            Format.R8G8_UInt => Silk.NET.Vulkan.Format.R8G8Uint,
            Format.R8G8_SNorm => Silk.NET.Vulkan.Format.R8G8SNorm,
            Format.R8G8_SInt => Silk.NET.Vulkan.Format.R8G8Sint,
            Format.R8G8B8A8_UNorm => Silk.NET.Vulkan.Format.R8G8B8A8Unorm,
            Format.R8G8B8A8_UNorm_SRGB => Silk.NET.Vulkan.Format.R8G8B8A8Srgb,
            Format.R8G8B8A8_UInt => Silk.NET.Vulkan.Format.R8G8B8A8Uint,
            Format.R8G8B8A8_SNorm => Silk.NET.Vulkan.Format.R8G8B8A8SNorm,
            Format.R8G8B8A8_SInt => Silk.NET.Vulkan.Format.R8G8B8A8Sint,
            Format.B8G8R8A8_UNorm => Silk.NET.Vulkan.Format.B8G8R8A8Unorm,
            Format.B8G8R8A8_UNorm_SRGB => Silk.NET.Vulkan.Format.B8G8R8A8Srgb,
            //Format.R10G10B10A2_UNorm => Silk.NET.Vulkan.Format.R10G10B10A2Unorm,
            //Format.R10G10B10A2_UInt => Silk.NET.Vulkan.Format.R10G10B10A2Uint,
            //Format.R11G11B10_Float => Silk.NET.Vulkan.Format.R11G11B10Sfloat,
            Format.R16_Float => Silk.NET.Vulkan.Format.R16Sfloat,
            Format.D16_UNorm => Silk.NET.Vulkan.Format.D16Unorm,
            Format.R16_UNorm => Silk.NET.Vulkan.Format.R16Unorm,
            Format.R16_UInt => Silk.NET.Vulkan.Format.R16Uint,
            Format.R16_SNorm => Silk.NET.Vulkan.Format.R16SNorm,
            Format.R16_SInt => Silk.NET.Vulkan.Format.R16Sint,
            Format.R16G16_Float => Silk.NET.Vulkan.Format.R16G16Sfloat,
            Format.R16G16_UNorm => Silk.NET.Vulkan.Format.R16G16Unorm,
            Format.R16G16_UInt => Silk.NET.Vulkan.Format.R16G16Uint,
            Format.R16G16_SNorm => Silk.NET.Vulkan.Format.R16G16SNorm,
            Format.R16G16_SInt => Silk.NET.Vulkan.Format.R16G16Sint,
            Format.R16G16B16A16_Float => Silk.NET.Vulkan.Format.R16G16B16A16Sfloat,
            Format.R16G16B16A16_UNorm => Silk.NET.Vulkan.Format.R16G16B16A16Unorm,
            Format.R16G16B16A16_UInt => Silk.NET.Vulkan.Format.R16G16B16A16Uint,
            Format.R16G16B16A16_SNorm => Silk.NET.Vulkan.Format.R16G16B16A16SNorm,
            Format.R16G16B16A16_SInt => Silk.NET.Vulkan.Format.R16G16B16A16Sint,
            Format.R32_Float => Silk.NET.Vulkan.Format.R32Sfloat,
            Format.R32_UInt => Silk.NET.Vulkan.Format.R32Uint,
            Format.R32_SInt => Silk.NET.Vulkan.Format.R32Sint,
            Format.R32G32_Float => Silk.NET.Vulkan.Format.R32G32Sfloat,
            Format.R32G32_UInt => Silk.NET.Vulkan.Format.R32G32Uint,
            Format.R32G32_SInt => Silk.NET.Vulkan.Format.R32G32Sint,
            Format.R32G32B32_Float => Silk.NET.Vulkan.Format.R32G32B32Sfloat,
            Format.R32G32B32_UInt => Silk.NET.Vulkan.Format.R32G32B32Uint,
            Format.R32G32B32_SInt => Silk.NET.Vulkan.Format.R32G32B32Sint,
            Format.R32G32B32A32_Float => Silk.NET.Vulkan.Format.R32G32B32A32Sfloat,
            Format.R32G32B32A32_UInt => Silk.NET.Vulkan.Format.R32G32B32A32Uint,
            Format.R32G32B32A32_SInt => Silk.NET.Vulkan.Format.R32G32B32A32Sint,
            Format.D24_UNorm_S8_UInt => Silk.NET.Vulkan.Format.D24UnormS8Uint,
            Format.D32_Float => Silk.NET.Vulkan.Format.D32Sfloat,
            Format.BC1_UNorm => Silk.NET.Vulkan.Format.BC1RgbaUnormBlock,
            Format.BC1_UNorm_SRGB => Silk.NET.Vulkan.Format.BC1RgbaSrgbBlock,
            Format.BC2_UNorm => Silk.NET.Vulkan.Format.BC2UnormBlock,
            Format.BC2_UNorm_SRGB => Silk.NET.Vulkan.Format.BC2SrgbBlock,
            Format.BC3_UNorm => Silk.NET.Vulkan.Format.BC3UnormBlock,
            Format.BC3_UNorm_SRGB => Silk.NET.Vulkan.Format.BC3SrgbBlock,
            Format.BC4_UNorm => Silk.NET.Vulkan.Format.BC4UnormBlock,
            Format.BC4_SNorm => Silk.NET.Vulkan.Format.BC4SNormBlock,
            Format.BC5_UNorm => Silk.NET.Vulkan.Format.BC5UnormBlock,
            Format.BC5_SNorm => Silk.NET.Vulkan.Format.BC5SNormBlock,
            Format.BC6H_UF16 => Silk.NET.Vulkan.Format.BC6HUfloatBlock,
            Format.BC6H_SF16 => Silk.NET.Vulkan.Format.BC6HSfloatBlock,
            Format.BC7_UNorm => Silk.NET.Vulkan.Format.BC7UnormBlock,
            Format.BC7_UNorm_SRGB => Silk.NET.Vulkan.Format.BC7SrgbBlock,

            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static PresentModeKHR PresentModeToVk(PresentMode presentMode)
    {
        return presentMode switch
        {
            PresentMode.Immediate => PresentModeKHR.ImmediateKhr,
            PresentMode.VerticalSync => PresentModeKHR.FifoKhr,
            PresentMode.AdaptiveSync => PresentModeKHR.FifoRelaxedKhr,
            _ => throw new ArgumentOutOfRangeException(nameof(presentMode), presentMode, null)
        };
    }
}