global using VulkanInstance = Silk.NET.Vulkan.Instance;
using System.Diagnostics;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkInstance : Instance
{
    private readonly Vk _vk;

    private readonly VulkanInstance _instance;

    private readonly KhrSurface _khrSurface;

    private readonly ExtDebugUtils? _debugUtils;
    private readonly DebugUtilsMessengerEXT _debugMessenger;
    
    public override bool IsDisposed { get; protected set; }

    public override string BackendName => VulkanBackend.Name;

    public VkInstance(ref readonly InstanceInfo info)
    {
        _vk = Vk.GetApi();

        using Utf8String pAppName = info.AppName;
        using Utf8String pEngineName = "GRABS";

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = pAppName,
            ApplicationVersion = Vk.MakeVersion(1, 0),
            PEngineName = pEngineName,
            EngineVersion = Vk.MakeVersion(1, 0),
            ApiVersion = Vk.Version13
        };

        List<string> instanceExtensions = [KhrSurface.ExtensionName];
        List<string> layers = [];

        uint numInstanceExtensions;
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numInstanceExtensions, null);
        ExtensionProperties* extensionProperties = stackalloc ExtensionProperties[(int) numInstanceExtensions];
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numInstanceExtensions, extensionProperties);

        GrabsLog.Log("Available instance extensions:");
        
        for (uint i = 0; i < numInstanceExtensions; i++)
        {
            string extension = new string((sbyte*) extensionProperties[i].ExtensionName);
            GrabsLog.Log($"    {extension}");

            if (extension
                is KhrWin32Surface.ExtensionName
                or KhrXlibSurface.ExtensionName
                or KhrXcbSurface.ExtensionName
                or KhrWaylandSurface.ExtensionName)
            {
                instanceExtensions.Add(extension);
            }
        }
        

        // TODO: Perform checks for these.
        if (info.Debug)
        {
            instanceExtensions.Add(ExtDebugUtils.ExtensionName);
            layers.Add("VK_LAYER_KHRONOS_validation");
        }

        GrabsLog.Log(GrabsLog.Severity.Debug,
            $"Enabled instance extensions: [{string.Join(", ", instanceExtensions)}]");
        
        GrabsLog.Log(GrabsLog.Severity.Debug, $"Enabled layers: [{string.Join(", ", layers)}]");

        using Utf8Array pInstanceExtensions = new Utf8Array(instanceExtensions);
        using Utf8Array pLayers = new Utf8Array(layers);

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = pInstanceExtensions.Length,
            PpEnabledExtensionNames = pInstanceExtensions,
            
            EnabledLayerCount = pLayers.Length,
            PpEnabledLayerNames = pLayers
        };
        
        GrabsLog.Log("Creating instance");
        _vk.CreateInstance(&instanceInfo, null, out _instance).Check("Create instance");

        if (!_vk.TryGetInstanceExtension(_instance, out _khrSurface))
            throw new Exception("Failed to get Surface extension.");

        if (info.Debug)
        {
            GrabsLog.Log("Getting debug utils extension.");
            if (!_vk.TryGetInstanceExtension(_instance, out _debugUtils))
                throw new Exception("Failed to get debug utils extension");
            
            Debug.Assert(_debugUtils != null);

            DebugUtilsMessengerCreateInfoEXT messengerInfo = new()
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

            GrabsLog.Log("Creating debug messenger.");
            _debugUtils.CreateDebugUtilsMessenger(_instance, &messengerInfo, null, out _debugMessenger)
                .Check("Create debug messenger");
        }
    }

    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = [];

        uint numDevices;
        _vk.EnumeratePhysicalDevices(_instance, &numDevices, null);
        PhysicalDevice* physicalDevices = stackalloc PhysicalDevice[(int) numDevices];
        _vk.EnumeratePhysicalDevices(_instance, &numDevices, physicalDevices);

        for (uint i = 0; i < numDevices; i++)
        {
            PhysicalDevice device = physicalDevices[i];

            PhysicalDeviceProperties props;
            _vk.GetPhysicalDeviceProperties(device, &props);

            PhysicalDeviceMemoryProperties memProps;
            _vk.GetPhysicalDeviceMemoryProperties(device, &memProps);

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

            // TODO: Use VMA to get the amount of memory?
            ulong dedicatedMemory = memProps.MemoryHeaps[0].Size;
            
            adapters.Add(new Adapter(device.Handle, i, name, type, dedicatedMemory));
        }
        
        return adapters.ToArray();
    }

    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new VkSurface(_vk, _khrSurface, _instance, in info);
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        Adapter pAdapter = adapter ?? EnumerateAdapters()[0];
        PhysicalDevice physicalDevice = new PhysicalDevice(pAdapter.Handle);

        return new VkDevice(_vk, _instance, _khrSurface, physicalDevice, ((VkSurface) surface).Surface);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;
        
        ResourceTracker.DisposeAllInstanceResources(_instance);

        if (_debugUtils != null)
        {
            GrabsLog.Log("Destroying debug messenger");
            _debugUtils.DestroyDebugUtilsMessenger(_instance, _debugMessenger, null);
            _debugUtils.Dispose();
        }
        
        _khrSurface.Dispose();
        
        GrabsLog.Log("Destroying instance");
        _vk.DestroyInstance(_instance, null);
        _vk.Dispose();
    }
    
    private static uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        string message = new string((sbyte*) pCallbackData->PMessage);

        if (messageSeverity == DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt)
            throw new Exception(message);

        GrabsLog.Severity severity = messageSeverity switch
        {
            DebugUtilsMessageSeverityFlagsEXT.None => GrabsLog.Severity.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt => GrabsLog.Severity.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.InfoBitExt => GrabsLog.Severity.Verbose,
            DebugUtilsMessageSeverityFlagsEXT.WarningBitExt => GrabsLog.Severity.Warning,
            DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt => GrabsLog.Severity.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(messageSeverity), messageSeverity, null)
        };
        
        GrabsLog.Log(severity, $"{messageTypes.ToString()[..^("BitExt".Length)]} - {message}");
        
        return Vk.True;
    }
}