using System.Diagnostics;
using System.Runtime.CompilerServices;
using grabs.Core;
using grabs.VulkanMemoryAllocator;
using Silk.NET.Vulkan;
using static grabs.VulkanMemoryAllocator.VmaAllocationCreateFlagBits;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanTexture : Texture
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly VmaAllocator_T* _allocator;
    private readonly bool _isVmaAllocated;

    public VmaAllocation_T* Allocation;

    public readonly Image Image;
    public readonly ImageView ImageView;
    
    public override Size2D Size { get; }

    public VulkanTexture(Vk vk, VulkanDevice device, VmaAllocator_T* allocator, ref readonly TextureInfo info, void* pData)
    {
        _vk = vk;
        _device = device.Device;
        _allocator = allocator;
        _isVmaAllocated = true;
        
        ImageType imageType = info.Type switch
        {
            TextureType.Texture1D => ImageType.Type1D,
            TextureType.Texture2D => ImageType.Type2D,
            TextureType.Texture3D => ImageType.Type3D,
            _ => throw new ArgumentOutOfRangeException()
        };

        ImageUsageFlags usage = ImageUsageFlags.TransferDstBit;
        if (info.Usage.HasFlag(TextureUsage.Sampled))
            usage |= ImageUsageFlags.SampledBit;
        
        ImageCreateInfo imageInfo = new ImageCreateInfo()
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = imageType,
            Extent = info.Size.ToVk(),
            Format = info.Format.ToVk(),
            Samples = SampleCountFlags.Count1Bit,
            Usage = usage,
            Tiling = ImageTiling.Optimal,
            ArrayLayers = 1,
            MipLevels = 1,
            InitialLayout = ImageLayout.Undefined,
            SharingMode = SharingMode.Exclusive
        };

        VmaAllocationCreateInfo allocInfo = new VmaAllocationCreateInfo()
        {
            usage = VmaMemoryUsage.VMA_MEMORY_USAGE_AUTO_PREFER_DEVICE,
            flags = 0
        };

        Vma.CreateImage(allocator, &imageInfo, &allocInfo, out Image, out Allocation, out _)
            .Check("Create image");

        if (pData != null)
        {
            uint dataSize = info.Size.Width * info.Size.Height * info.Size.Depth * info.Format.BytesPerPixel();

            BufferCreateInfo bufferInfo = new BufferCreateInfo()
            {
                SType = StructureType.BufferCreateInfo,
                Usage = BufferUsageFlags.TransferSrcBit,
                Size = dataSize,
                SharingMode = SharingMode.Exclusive
            };

            VmaAllocationCreateInfo bufferAllocInfo = new VmaAllocationCreateInfo()
            {
                usage = VmaMemoryUsage.VMA_MEMORY_USAGE_AUTO,
                flags = (uint) (VMA_ALLOCATION_CREATE_MAPPED_BIT | VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT)
            };

            Vma.CreateBuffer(allocator, &bufferInfo, &bufferAllocInfo, out VkBuffer transferBuffer,
                out VmaAllocation_T* allocation, out VmaAllocationInfo allocationInfo).Check("Create transfer buffer");

            Unsafe.CopyBlock(allocationInfo.pMappedData, pData, dataSize);

            CommandBuffer cb = device.BeginCommands();
            VulkanUtils.ImageBarrier(_vk, cb, Image, ImageLayout.Undefined, ImageLayout.TransferDstOptimal);

            BufferImageCopy biCopy = new BufferImageCopy()
            {
                BufferOffset = 0,
                ImageExtent = imageInfo.Extent,
                ImageSubresource = new ImageSubresourceLayers()
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    LayerCount = 1
                }
            };
            _vk.CmdCopyBufferToImage(cb, transferBuffer, Image, ImageLayout.TransferDstOptimal, 1, &biCopy);

            VulkanUtils.ImageBarrier(_vk, cb, Image, ImageLayout.TransferDstOptimal, ImageLayout.ShaderReadOnlyOptimal);
            
            device.EndCommands();
            
            Vma.DestroyBuffer(_allocator, transferBuffer, allocation);
        }
        else
            throw new NotImplementedException("Textures must be created with data.");
    }

    public VulkanTexture(Vk vk, VkDevice device, Image image, ImageView view, Size2D size)
    {
        _vk = vk;
        _device = device;
        _isVmaAllocated = false;

        Image = image;
        ImageView = view;
        Size = size;
    }

    public override void Dispose()
    {
        _vk.DestroyImageView(_device, ImageView, null);
        
        if (_isVmaAllocated)
            Vma.DestroyImage(_allocator, Image, Allocation);
    }
}