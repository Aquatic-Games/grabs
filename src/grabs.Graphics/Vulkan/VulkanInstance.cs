global using VkInstance = Silk.NET.Vulkan.Instance;
using grabs.Core;
using grabs.Graphics.Exceptions;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanInstance : Instance
{
    private readonly ExtDebugUtils? _debugUtils;
    private readonly DebugUtilsMessengerEXT _debugMessenger;

    private readonly KhrSurface _khrSurface;
    
    public readonly Vk Vk;
    
    public readonly VkInstance Instance;

    public override Backend Backend => Backend.Vulkan;

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

        const string validationLayerName = "VK_LAYER_KHRONOS_validation";
        
        List<string> instanceExtensions = [KhrSurface.ExtensionName];
        List<string> layersList = [];
        
        GrabsLog.Log("Checking instance extensions.");
        uint numProperties;
        Vk.EnumerateInstanceExtensionProperties((byte*) null, &numProperties, null);
        ExtensionProperties[] properties = new ExtensionProperties[numProperties];
        fixed (ExtensionProperties* pProps = properties)
            Vk.EnumerateInstanceExtensionProperties((byte*) null, &numProperties, pProps);

        bool debugSupported = false;
        foreach (ExtensionProperties property in properties)
        {
            string name = new string((sbyte*) property.ExtensionName);

            switch (name)
            {
                case KhrWin32Surface.ExtensionName:
                    instanceExtensions.Add(KhrWin32Surface.ExtensionName);
                    break;
                case KhrXlibSurface.ExtensionName:
                    instanceExtensions.Add(KhrXlibSurface.ExtensionName);
                    break;
                case KhrXcbSurface.ExtensionName:
                    instanceExtensions.Add(KhrXcbSurface.ExtensionName);
                    break;
                case KhrWaylandSurface.ExtensionName:
                    instanceExtensions.Add(KhrWaylandSurface.ExtensionName);
                    break;

                case ExtDebugUtils.ExtensionName when info.Debug:
                {
                    GrabsLog.Log("Debug extension is found, checking for validation layer presence.");
                    
                    uint numLayers;
                    Vk.EnumerateInstanceLayerProperties(&numLayers, null);
                    LayerProperties[] layerProps = new LayerProperties[numLayers];
                    fixed (LayerProperties* pLayerProps = layerProps)
                        Vk.EnumerateInstanceLayerProperties(&numLayers, pLayerProps);

                    foreach (LayerProperties layerProperty in layerProps)
                    {
                        string layerName = new string((sbyte*) layerProperty.LayerName);

                        if (layerName == validationLayerName)
                        {
                            debugSupported = true;
                            break;
                        }
                    }
                    
                    break;
                }
            }
        }

        if (info.Debug)
        {
            if (!debugSupported)
            {
                throw new DebugLayersNotFoundException(
                    $"Debugging is enabled, but {ExtDebugUtils.ExtensionName} extension and/or {validationLayerName} layer are not present. Please ensure you have a valid Vulkan SDK of AT LEAST version 1.3. The latest Vulkan SDK can be downloaded here: https://vulkan.lunarg.com/");
            }

            GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Source.Performance,
                "Debugging is enabled. This will affect performance.");
            
            instanceExtensions.Add(ExtDebugUtils.ExtensionName);
            layersList.Add(validationLayerName);
        }

        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General,
            $"Instance exts: [{string.Join(", ", instanceExtensions)}]");

        GrabsLog.Log($"Layers: [{string.Join(", ", layersList)}]");

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

        GrabsLog.Log("Creating instance");
        Vk.CreateInstance(&instanceInfo, null, out Instance).Check("Create instance");

        if (info.Debug)
        {
            if (!Vk.TryGetInstanceExtension(Instance, out _debugUtils))
                throw new Exception("Failed to get Debug Utils extension.");

            DebugUtilsMessengerCreateInfoEXT debugInfo = new DebugUtilsMessengerCreateInfoEXT()
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

            GrabsLog.Log("Creating debug messenger.");
            _debugUtils!.CreateDebugUtilsMessenger(Instance, &debugInfo, null, out _debugMessenger)
                .Check("Create debug messenger");
        }

        if (!Vk.TryGetInstanceExtension(Instance, out _khrSurface))
            throw new Exception("Failed to get Surface extension.");
    }

    private uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        string message = new string((sbyte*) pCallbackData->PMessage);
        
        // TODO: ValidationException
        if (messageSeverity == DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
            throw new Exception(message);

        GrabsLog.Severity severity = messageSeverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.None => GrabsLog.Severity.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => GrabsLog.Severity.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => GrabsLog.Severity.Info,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => GrabsLog.Severity.Warning,
            DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => GrabsLog.Severity.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null)
        };

        GrabsLog.Source source = messageTypes switch
        {
            DebugUtilsMessageTypeFlagsEXT.None => GrabsLog.Source.General,
            DebugUtilsMessageTypeFlagsEXT.GeneralBitExt => GrabsLog.Source.General,
            DebugUtilsMessageTypeFlagsEXT.ValidationBitExt => GrabsLog.Source.Validation,
            DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt => GrabsLog.Source.Performance,
            DebugUtilsMessageTypeFlagsEXT.DeviceAddressBindingBitExt => GrabsLog.Source.Other,
            _ => throw new ArgumentOutOfRangeException(nameof(messageTypes), messageTypes, null)
        };
        
        GrabsLog.Log(severity, source, message);

        return Vk.True;
    }

    public override Adapter[] EnumerateAdapters()
    {
        uint numDevices;
        Vk.EnumeratePhysicalDevices(Instance, &numDevices, null);
        PhysicalDevice* devices = stackalloc PhysicalDevice[(int) numDevices];
        Vk.EnumeratePhysicalDevices(Instance, &numDevices, devices);

        List<Adapter> adapters = new List<Adapter>();

        for (uint i = 0; i < numDevices; i++)
        {
            if (!CheckPhysicalDeviceSupported(devices[i]))
                continue;
            
            PhysicalDeviceProperties props;
            Vk.GetPhysicalDeviceProperties(devices[i], &props);
            
            PhysicalDeviceMemoryProperties memProps;
            Vk.GetPhysicalDeviceMemoryProperties(devices[i], &memProps);

            string name = new string((sbyte*) props.DeviceName);

            AdapterType type = props.DeviceType switch
            {
                PhysicalDeviceType.Other => AdapterType.Other,
                PhysicalDeviceType.IntegratedGpu => AdapterType.Integrated,
                PhysicalDeviceType.DiscreteGpu => AdapterType.Dedicated,
                PhysicalDeviceType.VirtualGpu => AdapterType.Other,
                PhysicalDeviceType.Cpu => AdapterType.Software,
                _ => throw new ArgumentOutOfRangeException()
            };

            ulong dedicatedMemory = memProps.MemoryHeaps[0].Size;

            adapters.Add(new Adapter(devices[i].Handle, i, name, type, dedicatedMemory));
        }
        
        if (adapters.Count == 0)
            throw new NotSupportedException("No adapters supporting Vulkan 1.3 or VK_KHR_dynamic_rendering are present.");

        return adapters.ToArray();
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        PhysicalDevice physicalDevice;

        if (adapter?.Handle is { } handle)
            physicalDevice = new PhysicalDevice(handle);
        else
        {
            Adapter[] adapters = EnumerateAdapters();
            physicalDevice = new PhysicalDevice(adapters[0].Handle);
        }

        return new VulkanDevice(Vk, Instance, physicalDevice, (VulkanSurface) surface, _khrSurface);
    }

    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new VulkanSurface(Vk, Instance, _khrSurface, in info);
    }

    private bool CheckPhysicalDeviceSupported(PhysicalDevice device)
    {
        uint numProperties;
        Vk.EnumerateDeviceExtensionProperties(device, (byte*) null, &numProperties, null);
        ExtensionProperties[] extensions = new ExtensionProperties[numProperties];
        fixed (ExtensionProperties* pProperties = extensions)
            Vk.EnumerateDeviceExtensionProperties(device, (byte*) null, &numProperties, pProperties);

        bool supported = false;
        foreach (ExtensionProperties property in extensions)
        {
            if (new string((sbyte*) property.ExtensionName) == "VK_KHR_dynamic_rendering")
            {
                supported = true;
                break;
            }
        }
        
        return supported;
    }

    public override void Dispose()
    {
        _khrSurface.Dispose();
        
        if (_debugUtils != null)
        {
            _debugUtils.DestroyDebugUtilsMessenger(Instance, _debugMessenger, null);
            _debugUtils.Dispose();
        }
        
        Vk.DestroyInstance(Instance, null);
        
        Vk.Dispose();
    }
}