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

    public static Result CreateBuffer(VmaAllocator_T* allocator, BufferCreateInfo* pBufferCreateInfo, VmaAllocationCreateInfo* pAllocationCreateInfo, out Buffer buffer, out VmaAllocation_T* allocation, out VmaAllocationInfo allocationInfo)
    {
        fixed (Buffer* pBuffer = &buffer)
        fixed (VmaAllocation_T** pAllocation = &allocation)
        fixed (VmaAllocationInfo* pAllocationInfo = &allocationInfo)
        {
            return CreateBuffer(allocator, pBufferCreateInfo, pAllocationCreateInfo, pBuffer, pAllocation,
                pAllocationInfo);
        }
    }
}