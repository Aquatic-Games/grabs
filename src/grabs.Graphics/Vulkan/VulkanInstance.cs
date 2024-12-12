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
        Vk.DestroyInstance(Instance, null);
        Vk.Dispose();
    }
}