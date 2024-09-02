using System;
using Silk.NET.Vulkan;
using static grabs.Graphics.Vulkan.VkResult;

namespace grabs.Graphics.Vulkan;

public sealed unsafe class VkSwapchainTexture : Texture
{
    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    
    public readonly Image[] Images;
    
    public readonly ImageView[] Views;

    public VkSwapchainTexture(Vk vk, VulkanDevice device, in ReadOnlySpan<Image> images, Format format, uint width, uint height) 
        : base(TextureDescription.Texture2D(width, height, 1, format, TextureUsage.None))
    {
        _vk = vk;
        _device = device;

        Images = images.ToArray();
        Views = new ImageView[images.Length];

        for (int i = 0; i < Views.Length; i++)
        {
            ImageViewCreateInfo viewInfo = new ImageViewCreateInfo()
            {
                SType = StructureType.ImageViewCreateInfo,

                Image = images[i],
                ViewType = ImageViewType.Type2D,
                Format = VkUtils.FormatToVk(format),

                Components = new ComponentMapping()
                {
                    R = ComponentSwizzle.Identity,
                    G = ComponentSwizzle.Identity,
                    B = ComponentSwizzle.Identity,
                    A = ComponentSwizzle.Identity
                },

                SubresourceRange = new ImageSubresourceRange()
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    BaseMipLevel = 0,
                    LevelCount = 1,
                    BaseArrayLayer = 0,
                    LayerCount = 1
                }
            };

            CheckResult(_vk.CreateImageView(_device, &viewInfo, null, out Views[i]), "Create swapchain image view");
        }
    }
    
    public override void Dispose()
    {
        for (int i = 0; i < Images.Length; i++)
            _vk.DestroyImageView(_device, Views[i], null);
        
        // Cannot destroy the actual images themselves as they are owned by the swapchain.
    }
}