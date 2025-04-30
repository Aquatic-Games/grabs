global using VulkanFormat = Silk.NET.Vulkan.Format;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal static class VkUtils
{
    public static void Check(this Result result, string operation)
    {
        if (result != Result.Success)
            throw new Exception($"Vulkan operation '{operation}' failed: {result}");
    }

    public static VulkanFormat ToVk(this Format format)
    {
        return format switch
        {
            Format.Unknown => VulkanFormat.Undefined,
            Format.B5G6R5_UNorm => VulkanFormat.B5G6R5UnormPack16,
            Format.B5G5R5A1_UNorm => VulkanFormat.B5G5R5A1UnormPack16,
            Format.R8_UNorm => VulkanFormat.R8Unorm,
            Format.R8_UInt => VulkanFormat.R8Uint,
            Format.R8_SNorm => VulkanFormat.R8SNorm,
            Format.R8_SInt => VulkanFormat.R8Sint,
            Format.A8_UNorm => VulkanFormat.A8UnormKhr,
            Format.R8G8_UNorm => VulkanFormat.R8G8Unorm,
            Format.R8G8_UInt => VulkanFormat.R8G8Uint,
            Format.R8G8_SNorm => VulkanFormat.R8G8SNorm,
            Format.R8G8_SInt => VulkanFormat.R8G8Sint,
            Format.R8G8B8A8_UNorm => VulkanFormat.R8G8B8A8Unorm,
            Format.R8G8B8A8_UNorm_SRGB => VulkanFormat.R8G8B8A8Srgb,
            Format.R8G8B8A8_UInt => VulkanFormat.R8G8B8A8Uint,
            Format.R8G8B8A8_SNorm => VulkanFormat.R8G8B8A8SNorm,
            Format.R8G8B8A8_SInt => VulkanFormat.R8G8B8A8Sint,
            Format.B8G8R8A8_UNorm => VulkanFormat.B8G8R8A8Unorm,
            Format.B8G8R8A8_UNorm_SRGB => VulkanFormat.B8G8R8A8Srgb,
            Format.R16_Float => VulkanFormat.R16Sfloat,
            Format.D16_UNorm => VulkanFormat.D16Unorm,
            Format.R16_UNorm => VulkanFormat.R16Unorm,
            Format.R16_UInt => VulkanFormat.R16Uint,
            Format.R16_SNorm => VulkanFormat.R16SNorm,
            Format.R16_SInt => VulkanFormat.R16Sint,
            Format.R16G16_Float => VulkanFormat.R16G16Sfloat,
            Format.R16G16_UNorm => VulkanFormat.R16G16Unorm,
            Format.R16G16_UInt => VulkanFormat.R16G16Uint,
            Format.R16G16_SNorm => VulkanFormat.R16G16SNorm,
            Format.R16G16_SInt => VulkanFormat.R16G16Sint,
            Format.R16G16B16A16_Float => VulkanFormat.R16G16B16A16Sfloat,
            Format.R16G16B16A16_UNorm => VulkanFormat.R16G16B16A16Unorm,
            Format.R16G16B16A16_UInt => VulkanFormat.R16G16B16A16Uint,
            Format.R16G16B16A16_SNorm => VulkanFormat.R16G16B16A16SNorm,
            Format.R16G16B16A16_SInt => VulkanFormat.R16G16B16A16Sint,
            Format.R32_Float => VulkanFormat.R32Sfloat,
            Format.R32_UInt => VulkanFormat.R32Uint,
            Format.R32_SInt => VulkanFormat.R32Sint,
            Format.R32G32_Float => VulkanFormat.R32G32Sfloat,
            Format.R32G32_UInt => VulkanFormat.R32G32Uint,
            Format.R32G32_SInt => VulkanFormat.R32G32Sint,
            Format.R32G32B32_Float => VulkanFormat.R32G32B32Sfloat,
            Format.R32G32B32_UInt => VulkanFormat.R32G32B32Uint,
            Format.R32G32B32_SInt => VulkanFormat.R32G32B32Sint,
            Format.R32G32B32A32_Float => VulkanFormat.R32G32B32A32Sfloat,
            Format.R32G32B32A32_UInt => VulkanFormat.R32G32B32A32Uint,
            Format.R32G32B32A32_SInt => VulkanFormat.R32G32B32A32Sint,
            Format.D24_UNorm_S8_UInt => VulkanFormat.D24UnormS8Uint,
            Format.D32_Float => VulkanFormat.D32Sfloat,
            Format.BC1_UNorm => VulkanFormat.BC1RgbaUnormBlock,
            Format.BC1_UNorm_SRGB => VulkanFormat.BC1RgbaSrgbBlock,
            Format.BC2_UNorm => VulkanFormat.BC2UnormBlock,
            Format.BC2_UNorm_SRGB => VulkanFormat.BC2SrgbBlock,
            Format.BC3_UNorm => VulkanFormat.BC3UnormBlock,
            Format.BC3_UNorm_SRGB => VulkanFormat.BC3SrgbBlock,
            Format.BC4_UNorm => VulkanFormat.BC4UnormBlock,
            Format.BC4_SNorm => VulkanFormat.BC4SNormBlock,
            Format.BC5_UNorm => VulkanFormat.BC5UnormBlock,
            Format.BC5_SNorm => VulkanFormat.BC5SNormBlock,
            Format.BC6H_UF16 => VulkanFormat.BC6HUfloatBlock,
            Format.BC6H_SF16 => VulkanFormat.BC6HSfloatBlock,
            Format.BC7_UNorm => VulkanFormat.BC7UnormBlock,
            Format.BC7_UNorm_SRGB => VulkanFormat.BC7SrgbBlock,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static PresentModeKHR ToVk(this PresentMode mode)
    {
        return mode switch
        {
            PresentMode.Fifo => PresentModeKHR.FifoKhr,
            PresentMode.Immediate => PresentModeKHR.ImmediateKhr,
            PresentMode.FifoRelaxed => PresentModeKHR.FifoRelaxedKhr,
            PresentMode.Mailbox => PresentModeKHR.MailboxKhr,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    public static AttachmentLoadOp ToVk(this LoadOp op)
    {
        return op switch
        {
            LoadOp.Clear => AttachmentLoadOp.Clear,
            LoadOp.Load => AttachmentLoadOp.Load,
            LoadOp.DontCare => AttachmentLoadOp.DontCare,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    public static AttachmentStoreOp ToVk(this StoreOp op)
    {
        return op switch
        {
            StoreOp.Store => AttachmentStoreOp.Store,
            StoreOp.DontCare => AttachmentStoreOp.DontCare,
            _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
        };
    }

    public static Extent2D ToVk(this Size2D size)
        => new Extent2D(size.Width, size.Height);
}