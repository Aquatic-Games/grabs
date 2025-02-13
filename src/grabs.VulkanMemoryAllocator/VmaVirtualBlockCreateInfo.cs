using Silk.NET.Vulkan;

namespace grabs.VulkanMemoryAllocator
{
    public unsafe partial struct VmaVirtualBlockCreateInfo
    {
        [NativeTypeName("VkDeviceSize")]
        public nuint size;

        [NativeTypeName("VmaVirtualBlockCreateFlags")]
        public uint flags;

        [NativeTypeName("const VkAllocationCallbacks * _Nullable")]
        public AllocationCallbacks* pAllocationCallbacks;
    }
}
