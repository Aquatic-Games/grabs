global using VulkanInstance = Silk.NET.Vulkan.Instance;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkInstance : Instance
{
    private readonly Vk _vk;
    
    public readonly VulkanInstance Instance;
    
    public override bool IsDisposed { get; protected set; }

    public override Backend Backend => Backend.Vulkan;

    public VkInstance(bool debug, IWindowProvider provider)
    {
        _vk = Vk.GetApi();

        using PinnedString appName = new PinnedString("GRABS application");
        using PinnedString engineName = new PinnedString("GRABS");

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            
            PApplicationName = (byte*) appName,
            ApplicationVersion = Vk.MakeVersion(1, 0),
            
            PEngineName = (byte*) engineName,
            EngineVersion = Vk.MakeVersion(1, 0),
            
            ApiVersion = Vk.Version13
        };
        
        InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            
            
        }
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}