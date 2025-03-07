using grabs.Core;
using grabs.VulkanMemoryAllocator;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanTexture : Texture
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly bool _destroyImage;

    public readonly Image Image;
    public readonly ImageView ImageView;
    
    public override Size2D Size { get; }

    public VulkanTexture(Vk vk, VmaAllocator_T* allocator, ref readonly TextureInfo info, void* pData)
    {
        ImageCreateInfo imageInfo = new ImageCreateInfo()
        {
            SType = StructureType.ImageCreateInfo,
            
        }
    }

    public VulkanTexture(Vk vk, VkDevice device, Image image, ImageView view, Size2D size)
    {
        _vk = vk;
        _device = device;
        _destroyImage = false;

        Image = image;
        ImageView = view;
        Size = size;
    }

    public override void Dispose()
    {
        _vk.DestroyImageView(_device, ImageView, null);
        
        if (_destroyImage)
            _vk.DestroyImage(_device, Image, null);
    }
}