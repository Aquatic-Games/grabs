global using VulkanInstance = Silk.NET.Vulkan.Instance;

using System;
using System.Collections.Generic;
using System.Reflection;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using static grabs.Graphics.Vulkan.VkUtils;

namespace grabs.Graphics.Vulkan;

public unsafe class VkInstance : Instance
{
    public readonly Vk Vk;

    public VulkanInstance Instance;

    public ExtDebugUtils DebugUtils;
    public DebugUtilsMessengerEXT DebugMessenger;
    
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

        List<string> newExtensions = new List<string>(extensions);
        newExtensions.Add(ExtDebugUtils.ExtensionName);
        
        using PinnedStringArray pExtensions = new PinnedStringArray(newExtensions.ToArray());
        using PinnedStringArray pLayers = new PinnedStringArray("VK_LAYER_KHRONOS_validation");

        InstanceCreateInfo instanceCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions,
            
            EnabledLayerCount = pLayers.Length,
            PpEnabledLayerNames = pLayers
        };

        GrabsLog.Log(GrabsLog.LogType.Verbose, "Creating instance.");
        CheckResult(Vk.CreateInstance(&instanceCreateInfo, null, out Instance), "create instance");

        if (!Vk.TryGetInstanceExtension(Instance, out DebugUtils))
            throw new Exception("Failed to get debug utils instance extension.");

        DebugUtilsMessengerCreateInfoEXT messengerCreateInfo = new DebugUtilsMessengerCreateInfoEXT()
        {
            SType = StructureType.DebugUtilsMessengerCreateInfoExt,

            MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                              DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                              DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                              DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
            MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                          DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                          DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,

            PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(DebugCallback)
        };
        
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Creating debug messenger.");
        CheckResult(DebugUtils.CreateDebugUtilsMessenger(Instance, &messengerCreateInfo, null, out DebugMessenger));
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
    
    private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        string message = messageTypes + " | " + new string((sbyte*) pCallbackData->PMessage);

        if (messageSeverity == DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
            throw new Exception(message);

        GrabsLog.LogType type = messageSeverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.None => GrabsLog.LogType.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => GrabsLog.LogType.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => GrabsLog.LogType.Info,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => GrabsLog.LogType.Warning,
            DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => GrabsLog.LogType.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null)
        };
        
        GrabsLog.Log(type, message);

        return Vk.True;
    }

    public override void Dispose()
    {
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Destroying debug messenger.");
        DebugUtils.DestroyDebugUtilsMessenger(Instance, DebugMessenger, null);
        DebugUtils.Dispose();
        
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Destroying instance.");
        Vk.DestroyInstance(Instance, null);
    }
}