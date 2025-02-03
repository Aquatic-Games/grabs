global using VkInstance = Silk.NET.Vulkan.Instance;

namespace grabs.Vulkan;

internal class VulkanInstance : Instance
{
    public readonly VkInstance Instance;

    public VulkanInstance(ref readonly InstanceInfo info)
    {
        
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}