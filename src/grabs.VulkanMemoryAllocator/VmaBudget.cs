using Silk.NET.Vulkan;

namespace grabs.VulkanMemoryAllocator
{
    public partial struct VmaBudget
    {
        public VmaStatistics statistics;

        [NativeTypeName("VkDeviceSize")]
        public nuint usage;

        [NativeTypeName("VkDeviceSize")]
        public nuint budget;
    }
}
