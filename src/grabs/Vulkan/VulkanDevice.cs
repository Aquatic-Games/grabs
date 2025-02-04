using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal sealed class VulkanDevice : Device
{
    private readonly Vk _vk;

    public VulkanDevice(Vk vk, VkInstance instance, PhysicalDevice physicalDevice)
    {
        _vk = vk;

        uint? graphicsQueueIndex;
        uint? presentQueueIndex;
        
        
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}