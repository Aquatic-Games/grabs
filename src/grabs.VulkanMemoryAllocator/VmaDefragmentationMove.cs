using Silk.NET.Vulkan;

namespace grabs.VulkanMemoryAllocator
{
    public unsafe partial struct VmaDefragmentationMove
    {
        public VmaDefragmentationMoveOperation operation;

        [NativeTypeName("VmaAllocation _Nonnull")]
        public VmaAllocation_T* srcAllocation;

        [NativeTypeName("VmaAllocation _Nonnull")]
        public VmaAllocation_T* dstTmpAllocation;
    }
}
