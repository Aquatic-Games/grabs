global using VulkanInstance = Silk.NET.Vulkan.Instance;

using System;
using System.Reflection;
using grabs.Core;
using Silk.NET.Vulkan;
using static grabs.Graphics.Vulkan.VkUtils;

namespace grabs.Graphics.Vulkan;

public unsafe class VkInstance : Instance
{
    public readonly Vk Vk;

    public VulkanInstance Instance;
    
    public override GraphicsApi Api => GraphicsApi.Vulkan;

    public VkInstance(string[] extensions, string appName = null, string engineName = null, Version appVersion = null, Version engineVersion = null)
    {
        Vk = Vk.GetApi();
        
        appName ??= Assembly.GetEntryAssembly()?.GetName().Name ?? "GRABS";
        engineName ??= "GRABS";
        uint vkAppVersion = appVersion != null
            ? Vk.MakeVersion((uint) appVersion.Major, (uint) appVersion.Major, (uint) appVersion.Build)
            : Vk.Version10;
        uint vkEngineVersion = engineVersion != null
            ? Vk.MakeVersion((uint) engineVersion.Major, (uint) engineVersion.Major, (uint) engineVersion.Build)
            : Vk.Version10;

        using PinnedString pAppName = new PinnedString(appName);
        using PinnedString pEngineName = new PinnedString(engineName);

        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            // TODO: Support Vulkan 1.0/1.1?
            ApiVersion = Vk.Version13,

            PApplicationName = pAppName,
            ApplicationVersion = vkAppVersion,
            
            PEngineName = pEngineName,
            EngineVersion = vkEngineVersion
        };

        using PinnedStringArray pExtensions = new PinnedStringArray(extensions);

        InstanceCreateInfo instanceCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions
        };

        GrabsLog.Log(GrabsLog.LogType.Verbose, "Creating instance.");
        CheckResult(Vk.CreateInstance(&instanceCreateInfo, null, out Instance), "create instance");
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override Adapter[] EnumerateAdapters()
    {
        uint numDevices;
        CheckResult(Vk.EnumeratePhysicalDevices(Instance, &numDevices, null));
        PhysicalDevice* pDevices = stackalloc PhysicalDevice[(int) numDevices];
        CheckResult(Vk.EnumeratePhysicalDevices(Instance, &numDevices, pDevices));

        Adapter[] adapters = new Adapter[numDevices];

        for (uint i = 0; i < numDevices; i++)
        {
            PhysicalDevice device = pDevices[i];
            
            PhysicalDeviceProperties properties;
            Vk.GetPhysicalDeviceProperties(device, &properties);

            PhysicalDeviceMemoryProperties memoryProperties;
            Vk.GetPhysicalDeviceMemoryProperties(device, &memoryProperties);

            string name = new string((sbyte*) properties.DeviceName);

            ulong dedicatedMemory = memoryProperties.MemoryHeapCount > 0
                ? memoryProperties.MemoryHeaps[0].Size
                : 0;
            
            AdapterType type = properties.DeviceType switch
            {
                PhysicalDeviceType.Other => AdapterType.Other,
                PhysicalDeviceType.IntegratedGpu => AdapterType.Integrated,
                PhysicalDeviceType.DiscreteGpu => AdapterType.Discrete,
                PhysicalDeviceType.VirtualGpu => AdapterType.Other,
                PhysicalDeviceType.Cpu => AdapterType.Software,
                _ => throw new ArgumentOutOfRangeException()
            };

            adapters[i] = new Adapter(i, name, dedicatedMemory, type);
        }

        return adapters;
    }

    public override void Dispose()
    {
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Destroying instance.");
        Vk.DestroyInstance(Instance, null);
    }
}