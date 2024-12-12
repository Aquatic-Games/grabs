global using VkInstance = Silk.NET.Vulkan.Instance;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using static grabs.Graphics.Vulkan.VulkanResult;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanInstance : Instance
{
    private readonly ExtDebugUtils? _debugUtils;

    private readonly DebugUtilsMessengerEXT _debugMessenger;
    
    public readonly Vk Vk;
    
    public readonly VkInstance Instance;
    
    public override bool IsDisposed { get; protected set; }
    
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
        string[] layers = [];

        if (debug)
        {
            Array.Resize(ref instanceExtensions, instanceExtensions.Length + 1);
            instanceExtensions[^1] = ExtDebugUtils.ExtensionName;

            layers = ["VK_LAYER_KHRONOS_validation"];
        }
        
        using PinnedStringArray pInstanceExtensions = new PinnedStringArray(instanceExtensions);
        using PinnedStringArray pLayers = new PinnedStringArray(layers);

        InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            
            PpEnabledExtensionNames = pInstanceExtensions,
            EnabledExtensionCount = pInstanceExtensions.Length,
            
            PpEnabledLayerNames = pLayers,
            EnabledLayerCount = pLayers.Length,
            
            PApplicationInfo = &appInfo
        };
        
        CheckResult(Vk.CreateInstance(&instanceInfo, null, out Instance), "Create instance");

        _debugUtils = null;
        _debugMessenger = default;

        if (debug)
        {
            if (!Vk.TryGetInstanceExtension(Instance, out _debugUtils!))
                throw new Exception("Failed to get debug utils extension.");

            DebugUtilsMessengerCreateInfoEXT messengerInfo = new DebugUtilsMessengerCreateInfoEXT()
            {
                SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.InfoBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                  DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt,
                MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                              DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                              DebugUtilsMessageTypeFlagsEXT.ValidationBitExt |
                              DebugUtilsMessageTypeFlagsEXT.DeviceAddressBindingBitExt,
                PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(DebugMessage)
            };

            CheckResult(_debugUtils.CreateDebugUtilsMessenger(Instance, &messengerInfo, null, out _debugMessenger),
                "Create debug messenger");
        }
    }

    public override Adapter[] EnumerateAdapters()
    {
        uint numDevices;
        Vk.EnumeratePhysicalDevices(Instance, &numDevices, null);
        PhysicalDevice* devices = stackalloc PhysicalDevice[(int) numDevices];
        Vk.EnumeratePhysicalDevices(Instance, &numDevices, devices);

        Adapter[] adapters = new Adapter[numDevices];

        for (uint i = 0; i < numDevices; i++)
        {
            PhysicalDevice device = devices[i];

            PhysicalDeviceProperties properties;
            Vk.GetPhysicalDeviceProperties(device, &properties);
            PhysicalDeviceMemoryProperties memoryProps;
            Vk.GetPhysicalDeviceMemoryProperties(device, &memoryProps);

            string name = new string((sbyte*) properties.DeviceName);
            ulong memory = memoryProps.MemoryHeaps[0].Size;

            AdapterType type = properties.DeviceType switch
            {
                PhysicalDeviceType.Other => AdapterType.Unknown,
                PhysicalDeviceType.IntegratedGpu => AdapterType.Integrated,
                PhysicalDeviceType.DiscreteGpu => AdapterType.Dedicated,
                PhysicalDeviceType.VirtualGpu => AdapterType.Unknown,
                PhysicalDeviceType.Cpu => AdapterType.Software,
                _ => throw new ArgumentOutOfRangeException()
            };

            adapters[i] = new Adapter(i, name, memory, type);
        }
        
        return adapters;
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        if (_debugUtils != null)
        {
            _debugUtils.DestroyDebugUtilsMessenger(Instance, _debugMessenger, null);
            _debugUtils.Dispose();
        }
        
        Vk.DestroyInstance(Instance, null);
        Vk.Dispose();
    }
    
    private static uint DebugMessage(DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        string message = new string((sbyte*) pCallbackData->PMessage);

        string type = messageTypes.ToString()[..^"BitExt".Length];
        if (messageSeverity.HasFlag(DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt))
            throw new Exception($"{type} error: {message}");

        string severity = messageSeverity.ToString()[..^"BitExt".Length];
        
        Console.WriteLine($"{severity} | {type}: {message}");

        return Vk.True;
    }
}