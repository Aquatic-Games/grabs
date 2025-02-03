global using VkInstance = Silk.NET.Vulkan.Instance;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal unsafe class VulkanInstance : Instance
{
    public readonly VkInstance Instance;

    public VulkanInstance(ref readonly InstanceInfo info)
    {
        using PinnedString appName = info.AppName;
        using PinnedString engineName = "GRABS";

        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,

            ApiVersion = Vk.Version13,

            PApplicationName = appName,
            ApplicationVersion = Vk.MakeVersion(1, 0),

            PEngineName = engineName,
            EngineVersion = Vk.MakeVersion(1, 0)
        };
        
        InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            
            PApplicationInfo = &appInfo,
            
        }
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}