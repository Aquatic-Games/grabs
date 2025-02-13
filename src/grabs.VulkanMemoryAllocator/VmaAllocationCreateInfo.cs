using Silk.NET.Vulkan;

namespace grabs.VulkanMemoryAllocator
{
    public unsafe partial struct VmaAllocationCreateInfo
    {
        [NativeTypeName("VmaAllocationCreateFlags")]
        public uint flags;

        public VmaMemoryUsage usage;

        [NativeTypeName("VkMemoryPropertyFlags")]
        public uint requiredFlags;

        [NativeTypeName("VkMemoryPropertyFlags")]
        public uint preferredFlags;

        [NativeTypeName("uint32_t")]
        public uint memoryTypeBits;

        [NativeTypeName("VmaPool _Nullable")]
        public VmaPool_T* pool;

        [NativeTypeName("void * _Nullable")]
        public void* pUserData;

        public float priority;
    }
}
