using Silk.NET.Vulkan;

namespace grabs.VulkanMemoryAllocator
{
    public partial struct VmaAllocationInfo2
    {
        public VmaAllocationInfo allocationInfo;

        [NativeTypeName("VkDeviceSize")]
        public nuint blockSize;

        [NativeTypeName("VkBool32")]
        public uint dedicatedMemory;
    }
}
