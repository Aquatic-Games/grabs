global using VkInstance = Silk.NET.Vulkan.Instance;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Vulkan;

internal unsafe class VulkanInstance : Instance
{
    public readonly Vk Vk;
    
    public readonly VkInstance Instance;

    public VulkanInstance(ref readonly InstanceInfo info)
    {
        Vk = Vk.GetApi();
        
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

        List<string> instanceExtensions = [KhrSurface.ExtensionName];
        List<string> layersList = [];

        if (OperatingSystem.IsWindows())
        {
            instanceExtensions.Add(KhrWin32Surface.ExtensionName);
        }
        else if (OperatingSystem.IsLinux())
        {
            instanceExtensions.Add(KhrXlibSurface.ExtensionName);
            instanceExtensions.Add(KhrXcbSurface.ExtensionName);
            instanceExtensions.Add(KhrWaylandSurface.ExtensionName);
        }
        else
            throw new PlatformNotSupportedException();

        if (info.Debug)
        {
            instanceExtensions.Add(ExtDebugUtils.ExtensionName);
            layersList.Add("VK_LAYER_KHRONOS_validation");
        }

        using PinnedStringArray extensions = new PinnedStringArray(instanceExtensions);
        using PinnedStringArray layers = new PinnedStringArray(layersList);

        InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,

            PApplicationInfo = &appInfo,

            PpEnabledExtensionNames = extensions,
            EnabledExtensionCount = extensions.Length,

            PpEnabledLayerNames = layers,
            EnabledLayerCount = layers.Length,
        };

        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.Trace, "Creating instance");
        Vk.CreateInstance(&instanceInfo, null, out Instance).Check("Create instance");
    }
    
    public override void Dispose()
    {
        Vk.DestroyInstance(Instance, null);
        
        Vk.Dispose();
    }
}