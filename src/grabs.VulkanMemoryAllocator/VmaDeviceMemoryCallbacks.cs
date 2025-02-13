using Silk.NET.Vulkan;

namespace grabs.VulkanMemoryAllocator
{
    public unsafe partial struct VmaDeviceMemoryCallbacks
    {
        [NativeTypeName("PFN_vmaAllocateDeviceMemoryFunction _Nullable")]
        public delegate* unmanaged[Cdecl]<VmaAllocator_T*, uint, DeviceMemory, nuint, void*, void> pfnAllocate;

        [NativeTypeName("PFN_vmaFreeDeviceMemoryFunction _Nullable")]
        public delegate* unmanaged[Cdecl]<VmaAllocator_T*, uint, DeviceMemory, nuint, void*, void> pfnFree;

        [NativeTypeName("void * _Nullable")]
        public void* pUserData;
    }
}
