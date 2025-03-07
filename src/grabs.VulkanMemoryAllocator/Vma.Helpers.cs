using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace grabs.VulkanMemoryAllocator;

public unsafe partial class Vma
{
    public static Result CreateAllocator(VmaAllocatorCreateInfo* pCreateInfo, out VmaAllocator_T* allocator)
    {
        fixed (VmaAllocator_T** pAllocator = &allocator)
            return CreateAllocator(pCreateInfo, pAllocator);
    }

    public static Result CreateBuffer(VmaAllocator_T* allocator, BufferCreateInfo* pBufferCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo, out Buffer buffer, out VmaAllocation_T* allocation,
        out VmaAllocationInfo allocationInfo)
    {
        fixed (Buffer* pBuffer = &buffer)
        fixed (VmaAllocation_T** pAllocation = &allocation)
        fixed (VmaAllocationInfo* pAllocationInfo = &allocationInfo)
        {
            return CreateBuffer(allocator, pBufferCreateInfo, pAllocationCreateInfo, pBuffer, pAllocation,
                pAllocationInfo);
        }
    }

    public static Result CreateImage(VmaAllocator_T* allocator, ImageCreateInfo* pImageCreateInfo,
        VmaAllocationCreateInfo* pAllocationCreateInfo, out Image image, out VmaAllocation_T* allocation,
        out VmaAllocationInfo allocationInfo)
    {
        fixed (Image* pImage = &image)
        fixed (VmaAllocation_T** pAllocation = &allocation)
        fixed (VmaAllocationInfo* pAllocationInfo = &allocationInfo)
        {
            return CreateImage(allocator, pImageCreateInfo, pAllocationCreateInfo, pImage, pAllocation,
                pAllocationInfo);
        }
    }
}