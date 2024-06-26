﻿global using VulkanInstance = Silk.NET.Vulkan.Instance;

using System;
using System.Collections.Generic;
using System.Reflection;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public unsafe class VkInstance : Instance
{
    public readonly Vk Vk;

    public readonly VulkanInstance Instance;

    public readonly ExtDebugUtils ExtDebugUtils;
    public readonly DebugUtilsMessengerEXT DebugMessenger;
    
    public readonly KhrSurface KhrSurface;
    
    public override GraphicsApi Api => GraphicsApi.Vulkan;

    public VkInstance(string[] instanceExtensions, bool debug = false)
    {
        Vk = Vk.GetApi();

        using PinnedString appName = new PinnedString(Assembly.GetEntryAssembly()?.GetName().Name ?? "GRABS Vulkan Application");
        using PinnedString engineName = new PinnedString("GRABSvk");
        
        Console.WriteLine($"appName: {appName}");
        Console.WriteLine($"engineName: {engineName}");
        
        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version13,
            
            PApplicationName = appName,
            ApplicationVersion = Vk.Version10,
            
            PEngineName = engineName,
            EngineVersion = Vk.Version10
        };
        
        GrabsLog.Log(GrabsLog.LogType.Debug, $"debug: {debug}");

        PinnedStringArray pExtensions;
        PinnedStringArray pLayers = null;
        if (debug)
        {
            List<string> iExtensions = new List<string>(instanceExtensions);
            iExtensions.Add(ExtDebugUtils.ExtensionName);
            pExtensions = new PinnedStringArray(iExtensions.ToArray());
            pLayers = new PinnedStringArray("VK_LAYER_KHRONOS_validation");
        }
        else
            pExtensions = new PinnedStringArray(instanceExtensions);
        
        GrabsLog.Log(GrabsLog.LogType.Debug, $"pExtensions: {pExtensions}");
        GrabsLog.Log(GrabsLog.LogType.Debug, $"pLayers: {pLayers?.ToString() ?? "null"}");

        InstanceCreateInfo iCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions,
            EnabledLayerCount = pLayers?.Length ?? 0,
            PpEnabledLayerNames = (byte**) pLayers?.Handle,
            PApplicationInfo = &appInfo
        };
        
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Creating instance.");
        Result result;
        if ((result = Vk.CreateInstance(&iCreateInfo, null, out Instance)) != Result.Success)
            throw new Exception($"Failed to create Vulkan instance: {result}");
        
        pLayers?.Dispose();
        pExtensions.Dispose();

        if (debug)
        {
            GrabsLog.Log(GrabsLog.LogType.Verbose, "Creating debug utils.");
            Vk.TryGetInstanceExtension(Instance, out ExtDebugUtils);

            DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new DebugUtilsMessengerCreateInfoEXT()
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.ValidationBitExt |
                              DebugUtilsMessageTypeFlagsEXT.DeviceAddressBindingBitExt,
                PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(DebugCallback)
            };
            
            if ((result = ExtDebugUtils.CreateDebugUtilsMessenger(Instance, &debugCreateInfo, null, out DebugMessenger)) != Result.Success)
                throw new Exception($"Failed to create debug messenger: {result}");
        }
        
        if (!Vk.TryGetInstanceExtension(Instance, out KhrSurface))
            throw new Exception("Could not get VK_KHR_surface extension. Is it supported?");
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        uint numDevices;
        Vk.EnumeratePhysicalDevices(Instance, &numDevices, null);
        Span<PhysicalDevice> devices = stackalloc PhysicalDevice[(int) numDevices];
        fixed (PhysicalDevice* pDevices = devices)
            Vk.EnumeratePhysicalDevices(Instance, &numDevices, pDevices);

        uint index = adapter?.Index ?? 0;
        return new VkDevice(Vk, Instance, KhrSurface, devices[(int) index], (VkSurface) surface);
    }

    public override Adapter[] EnumerateAdapters()
    {
        uint deviceCount;
        Vk.EnumeratePhysicalDevices(Instance, &deviceCount, null);
        Span<PhysicalDevice> devices = stackalloc PhysicalDevice[(int) deviceCount];
        fixed (PhysicalDevice* pDevices = devices)
            Vk.EnumeratePhysicalDevices(Instance, &deviceCount, pDevices);
        
        GrabsLog.Log(GrabsLog.LogType.Verbose, $"EnumerateAdapters: deviceCount: {deviceCount}");

        Adapter[] adapters = new Adapter[deviceCount];

        for (uint i = 0; i < deviceCount; i++)
        {
            PhysicalDevice device = devices[(int) i];
            
            PhysicalDeviceProperties deviceProps;
            Vk.GetPhysicalDeviceProperties(device, &deviceProps);

            AdapterType type = deviceProps.DeviceType switch
            {
                PhysicalDeviceType.Other => AdapterType.Other,
                PhysicalDeviceType.IntegratedGpu => AdapterType.Integrated,
                PhysicalDeviceType.DiscreteGpu => AdapterType.Discrete,
                PhysicalDeviceType.VirtualGpu => AdapterType.Other,
                PhysicalDeviceType.Cpu => AdapterType.Software,
                _ => throw new ArgumentOutOfRangeException()
            };

            PhysicalDeviceMemoryProperties memProps;
            Vk.GetPhysicalDeviceMemoryProperties(device, &memProps);

            ulong dedicatedMemory = memProps.MemoryHeaps[0].Size;

            adapters[i] = new Adapter(i, new string((sbyte*) deviceProps.DeviceName), dedicatedMemory, type);
        }
        
        return adapters;
    }

    public override void Dispose()
    {
        KhrSurface.Dispose();
        
        if (ExtDebugUtils != null)
        {
            ExtDebugUtils.DestroyDebugUtilsMessenger(Instance, DebugMessenger, null);
            ExtDebugUtils.Dispose();
        }
        
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Destroying instance.");
        Vk.DestroyInstance(Instance, null);
        
        Vk.Dispose();
    }
    
    private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageType, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        if (messageSeverity == DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
            throw new Exception($"DEBUG ERROR: {new string((sbyte*) pCallbackData->PMessage)}");

        GrabsLog.LogType type = messageSeverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.None => GrabsLog.LogType.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => GrabsLog.LogType.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => GrabsLog.LogType.Debug,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => GrabsLog.LogType.Warning,
            _ => throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null)
        };
        
        GrabsLog.Log(type, $"{messageType.ToString()[..^("BitExt".Length)]}: {new string((sbyte*) pCallbackData->PMessage)}");
        
        return Vk.True;
    }
}