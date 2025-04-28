using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkTexture : Texture
{
    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly bool _destroyImage;
    
    public readonly Image Image;
    
    public readonly ImageView ImageView;

    // Used in swapchain creation
    public VkTexture(Vk vk, VulkanDevice device, Image image, VkFormat format)
    {
        ResourceTracker.RegisterDeviceResource(device, this);
        
        _vk = vk;
        _device = device;
        _destroyImage = false;
        Image = image;

        ImageViewCreateInfo imageViewInfo = new()
        {
            SType = StructureType.ImageViewCreateInfo,
            Image = image,
            Format = format,
            ViewType = ImageViewType.Type2D,
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
                LevelCount = 1,
                BaseMipLevel = 0,
                LayerCount = 1,
                BaseArrayLayer = 0
            }
        };

        GrabsLog.Log("Creating image view");
        _vk.CreateImageView(_device, &imageViewInfo, null, out ImageView).Check("Create image view");
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        GrabsLog.Log("Destroying image view");
        _vk.DestroyImageView(_device, ImageView, null);

        if (_destroyImage)
        {
            GrabsLog.Log("Destroying image");
            _vk.DestroyImage(_device, Image, null);
        }

        ResourceTracker.DeregisterDeviceResource(_device, this);
    }
}