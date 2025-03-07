global using VkFormat = Silk.NET.Vulkan.Format;
global using VkDescriptorType = Silk.NET.Vulkan.DescriptorType;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal static class VulkanUtils
{
    public static void Check(this Result result, string operation)
    {
        if (result != Result.Success)
            throw new VulkanOperationException(operation, result);
    }
    
    public static unsafe void ImageBarrier(Vk vk, CommandBuffer buffer, Image image, ImageLayout old, ImageLayout @new)
    {
        ImageMemoryBarrier memoryBarrier = new ImageMemoryBarrier()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = image,
            OldLayout = old,
            NewLayout = @new,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit,
            SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1)
        };

        vk.CmdPipelineBarrier(buffer, PipelineStageFlags.ColorAttachmentOutputBit,
            PipelineStageFlags.ColorAttachmentOutputBit, 0, 0, null, 0, null, 1, &memoryBarrier);
    }
    
    public static VkFormat ToVk(this Format format)
    {
        return format switch
        {
            Format.B5G6R5_UNorm => VkFormat.B5G6R5UnormPack16,
            Format.B5G5R5A1_UNorm => VkFormat.B5G5R5A1UnormPack16,
            Format.R8_UNorm => VkFormat.R8Unorm,
            Format.R8_UInt => VkFormat.R8Uint,
            Format.R8_SNorm => VkFormat.R8SNorm,
            Format.R8_SInt => VkFormat.R8Sint,
            Format.A8_UNorm => VkFormat.A8UnormKhr,
            Format.R8G8_UNorm => VkFormat.R8G8Unorm,
            Format.R8G8_UInt => VkFormat.R8G8Uint,
            Format.R8G8_SNorm => VkFormat.R8G8SNorm,
            Format.R8G8_SInt => VkFormat.R8G8Sint,
            Format.R8G8B8A8_UNorm => VkFormat.R8G8B8A8Unorm,
            Format.R8G8B8A8_UNorm_SRGB => VkFormat.R8G8B8A8Srgb,
            Format.R8G8B8A8_UInt => VkFormat.R8G8B8A8Uint,
            Format.R8G8B8A8_SNorm => VkFormat.R8G8B8A8SNorm,
            Format.R8G8B8A8_SInt => VkFormat.R8G8B8A8Sint,
            Format.B8G8R8A8_UNorm => VkFormat.B8G8R8A8Unorm,
            Format.B8G8R8A8_UNorm_SRGB => VkFormat.B8G8R8A8Srgb,
            Format.R16_Float => VkFormat.R16Sfloat,
            Format.D16_UNorm => VkFormat.D16Unorm,
            Format.R16_UNorm => VkFormat.R16Unorm,
            Format.R16_UInt => VkFormat.R16Uint,
            Format.R16_SNorm => VkFormat.R16SNorm,
            Format.R16_SInt => VkFormat.R16Sint,
            Format.R16G16_Float => VkFormat.R16G16Sfloat,
            Format.R16G16_UNorm => VkFormat.R16G16Unorm,
            Format.R16G16_UInt => VkFormat.R16G16Uint,
            Format.R16G16_SNorm => VkFormat.R16G16SNorm,
            Format.R16G16_SInt => VkFormat.R16G16Sint,
            Format.R16G16B16A16_Float => VkFormat.R16G16B16A16Sfloat,
            Format.R16G16B16A16_UNorm => VkFormat.R16G16B16A16Unorm,
            Format.R16G16B16A16_UInt => VkFormat.R16G16B16A16Uint,
            Format.R16G16B16A16_SNorm => VkFormat.R16G16B16A16SNorm,
            Format.R16G16B16A16_SInt => VkFormat.R16G16B16A16Sint,
            Format.R32_Float => VkFormat.R32Sfloat,
            Format.R32_UInt => VkFormat.R32Uint,
            Format.R32_SInt => VkFormat.R32Sint,
            Format.R32G32_Float => VkFormat.R32G32Sfloat,
            Format.R32G32_UInt => VkFormat.R32G32Uint,
            Format.R32G32_SInt => VkFormat.R32G32Sint,
            Format.R32G32B32_Float => VkFormat.R32G32B32Sfloat,
            Format.R32G32B32_UInt => VkFormat.R32G32B32Uint,
            Format.R32G32B32_SInt => VkFormat.R32G32B32Sint,
            Format.R32G32B32A32_Float => VkFormat.R32G32B32A32Sfloat,
            Format.R32G32B32A32_UInt => VkFormat.R32G32B32A32Uint,
            Format.R32G32B32A32_SInt => VkFormat.R32G32B32A32Sint,
            Format.D24_UNorm_S8_UInt => VkFormat.D24UnormS8Uint,
            Format.D32_Float => VkFormat.D32Sfloat,
            Format.BC1_UNorm => VkFormat.BC1RgbaUnormBlock,
            Format.BC1_UNorm_SRGB => VkFormat.BC1RgbaSrgbBlock,
            Format.BC2_UNorm => VkFormat.BC2UnormBlock,
            Format.BC2_UNorm_SRGB => VkFormat.BC2SrgbBlock,
            Format.BC3_UNorm => VkFormat.BC3UnormBlock,
            Format.BC3_UNorm_SRGB => VkFormat.BC3SrgbBlock,
            Format.BC4_UNorm => VkFormat.BC4UnormBlock,
            Format.BC4_SNorm => VkFormat.BC4SNormBlock,
            Format.BC5_UNorm => VkFormat.BC5UnormBlock,
            Format.BC5_SNorm => VkFormat.BC5SNormBlock,
            Format.BC6H_UF16 => VkFormat.BC6HUfloatBlock,
            Format.BC6H_SF16 => VkFormat.BC6HSfloatBlock,
            Format.BC7_UNorm => VkFormat.BC7UnormBlock,
            Format.BC7_UNorm_SRGB => VkFormat.BC7SrgbBlock,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static Format ToGrabs(this VkFormat format)
    {
        return format switch
        {
            VkFormat.B5G6R5UnormPack16 => Format.B5G6R5_UNorm,
            VkFormat.B5G5R5A1UnormPack16 => Format.B5G5R5A1_UNorm,
            VkFormat.R8Unorm => Format.R8_UNorm,
            VkFormat.R8Uint => Format.R8_UInt,
            VkFormat.R8SNorm => Format.R8_SNorm,
            VkFormat.R8Sint => Format.R8_SInt,
            VkFormat.A8UnormKhr => Format.A8_UNorm,
            VkFormat.R8G8Unorm => Format.R8G8_UNorm,
            VkFormat.R8G8Uint => Format.R8G8_UInt,
            VkFormat.R8G8SNorm => Format.R8G8_SNorm,
            VkFormat.R8G8Sint => Format.R8G8_SInt,
            VkFormat.R8G8B8A8Unorm => Format.R8G8B8A8_UNorm,
            VkFormat.R8G8B8A8Srgb => Format.R8G8B8A8_UNorm_SRGB,
            VkFormat.R8G8B8A8Uint => Format.R8G8B8A8_UInt,
            VkFormat.R8G8B8A8SNorm => Format.R8G8B8A8_SNorm,
            VkFormat.R8G8B8A8Sint => Format.R8G8B8A8_SInt,
            VkFormat.B8G8R8A8Unorm => Format.B8G8R8A8_UNorm,
            VkFormat.B8G8R8A8Srgb => Format.B8G8R8A8_UNorm_SRGB,
            VkFormat.R16Sfloat => Format.R16_Float,
            VkFormat.D16Unorm => Format.D16_UNorm,
            VkFormat.R16Unorm => Format.R16_UNorm,
            VkFormat.R16Uint => Format.R16_UInt,
            VkFormat.R16SNorm => Format.R16_SNorm,
            VkFormat.R16Sint => Format.R16_SInt,
            VkFormat.R16G16Sfloat => Format.R16G16_Float,
            VkFormat.R16G16Unorm => Format.R16G16_UNorm,
            VkFormat.R16G16Uint => Format.R16G16_UInt,
            VkFormat.R16G16SNorm => Format.R16G16_SNorm,
            VkFormat.R16G16Sint => Format.R16G16_SInt,
            VkFormat.R16G16B16A16Sfloat => Format.R16G16B16A16_Float,
            VkFormat.R16G16B16A16Unorm => Format.R16G16B16A16_UNorm,
            VkFormat.R16G16B16A16Uint => Format.R16G16B16A16_UInt,
            VkFormat.R16G16B16A16SNorm => Format.R16G16B16A16_SNorm,
            VkFormat.R16G16B16A16Sint => Format.R16G16B16A16_SInt,
            VkFormat.R32Sfloat => Format.R32_Float,
            VkFormat.R32Uint => Format.R32_UInt,
            VkFormat.R32Sint => Format.R32_SInt,
            VkFormat.R32G32Sfloat => Format.R32G32_Float,
            VkFormat.R32G32Uint => Format.R32G32_UInt,
            VkFormat.R32G32Sint => Format.R32G32_SInt,
            VkFormat.R32G32B32Sfloat => Format.R32G32B32_Float,
            VkFormat.R32G32B32Uint => Format.R32G32B32_UInt,
            VkFormat.R32G32B32Sint => Format.R32G32B32_SInt,
            VkFormat.R32G32B32A32Sfloat => Format.R32G32B32A32_Float,
            VkFormat.R32G32B32A32Uint => Format.R32G32B32A32_UInt,
            VkFormat.R32G32B32A32Sint => Format.R32G32B32A32_SInt,
            VkFormat.D24UnormS8Uint => Format.D24_UNorm_S8_UInt,
            VkFormat.D32Sfloat => Format.D32_Float,
            VkFormat.BC1RgbaUnormBlock => Format.BC1_UNorm,
            VkFormat.BC1RgbaSrgbBlock => Format.BC1_UNorm_SRGB,
            VkFormat.BC2UnormBlock => Format.BC2_UNorm,
            VkFormat.BC2SrgbBlock => Format.BC2_UNorm_SRGB,
            VkFormat.BC3UnormBlock => Format.BC3_UNorm,
            VkFormat.BC3SrgbBlock => Format.BC3_UNorm_SRGB,
            VkFormat.BC4UnormBlock => Format.BC4_UNorm,
            VkFormat.BC4SNormBlock => Format.BC4_SNorm,
            VkFormat.BC5UnormBlock => Format.BC5_UNorm,
            VkFormat.BC5SNormBlock => Format.BC5_SNorm,
            VkFormat.BC6HUfloatBlock => Format.BC6H_UF16,
            VkFormat.BC6HSfloatBlock => Format.BC6H_SF16,
            VkFormat.BC7UnormBlock => Format.BC7_UNorm,
            VkFormat.BC7SrgbBlock => Format.BC7_UNorm_SRGB,
            _ => Format.Unknown
        };
    }

    public static AttachmentLoadOp ToVk(this LoadOp loadOp)
    {
        return loadOp switch
        {
            LoadOp.Clear => AttachmentLoadOp.Clear,
            LoadOp.Load => AttachmentLoadOp.Load,
            _ => throw new ArgumentOutOfRangeException(nameof(loadOp), loadOp, null)
        };
    }

    public static VkDescriptorType ToVk(this DescriptorType type)
    {
        return type switch
        {
            DescriptorType.ConstantBuffer => VkDescriptorType.UniformBuffer,
            DescriptorType.Texture => VkDescriptorType.SampledImage,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static ShaderStageFlags ToVk(this ShaderStage stage)
    {
        ShaderStageFlags flags = ShaderStageFlags.None;

        if (stage.HasFlag(ShaderStage.Vertex))
            flags |= ShaderStageFlags.VertexBit;
        if (stage.HasFlag(ShaderStage.Pixel))
            flags |= ShaderStageFlags.FragmentBit;

        return flags;
    }

    public static Extent2D ToVk(this Size2D size)
        => new Extent2D(size.Width, size.Height);

    public static Extent3D ToVk(this Size3D size)
        => new Extent3D(size.Width, size.Height, size.Depth);
}