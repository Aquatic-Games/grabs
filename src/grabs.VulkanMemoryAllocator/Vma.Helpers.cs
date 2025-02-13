using Silk.NET.Vulkan;

namespace grabs.VulkanMemoryAllocator;

public unsafe partial class Vma
{
    public static Result CreateAllocator(VmaAllocatorCreateInfo* pCreateInfo, out VmaAllocator_T* allocator)
    {
        fixed (VmaAllocator_T** pAllocator = &allocator)
            return CreateAllocator(pCreateInfo, pAllocator);
    }
}