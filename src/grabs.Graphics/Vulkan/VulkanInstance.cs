global using VkInstance = Silk.NET.Vulkan.Instance;
using grabs.Core;
using Silk.NET.Vulkan;
using static grabs.Graphics.Vulkan.VulkanResult;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanInstance : Instance
{
    public override bool IsDisposed { get; protected set; }

    public readonly Vk Vk;
    
    public readonly VkInstance Instance;
    
    public override Backend Backend => Backend.Vulkan;

    public VulkanInstance(bool debug, IWindowProvider provider)
    {
        Vk = Vk.GetApi();
        
        using PinnedString appName = new PinnedString("GRABS application");
        using PinnedString engineName = new PinnedString("GRABS");

        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,

            PApplicationName = appName,
            ApplicationVersion = Vk.MakeVersion(1, 0),

            PEngineName = engineName,
            EngineVersion = Vk.MakeVersion(1, 0),

            ApiVersion = Vk.Version13
        };

        string[] instanceExtensions = provider.GetVulkanInstanceExtensions();
        using PinnedStringArray pInstanceExtensions = new PinnedStringArray(instanceExtensions);

        InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            
            PpEnabledExtensionNames = pInstanceExtensions,
            EnabledExtensionCount = pInstanceExtensions.Length,
            
            PApplicationInfo = &appInfo
        };
        
        CheckResult(Vk.CreateInstance(&instanceInfo, null, out Instance), "Create instance");
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        Vk.DestroyInstance(Instance, null);
        Vk.Dispose();
    }
}