using System;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

public unsafe class VkInstance : Instance
{
    public Vk Vk;

    public Silk.NET.Vulkan.Instance Instance;

    public VkInstance(string[] instanceExtensions)
    {
        Vk = Vk.GetApi();
        
        // TODO: Probably don't need to use Vulkan 1.3. Just using it right now so dynamic rendering can be used without extensions.
        // TODO: Support both dynamic rendering and render passes.
        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version13
        };

        using PinnedStringArray pInstanceExtensions = new PinnedStringArray(instanceExtensions);
        
        InstanceCreateInfo iCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            EnabledExtensionCount = (uint) pInstanceExtensions.Length,
            PpEnabledExtensionNames = (byte**) pInstanceExtensions.Handle
        };

        Result result;
        if ((result = Vk.CreateInstance(&iCreateInfo, null, out Instance)) != Result.Success)
            throw new Exception($"Failed to create instance: {result}");
    }
    
    public override Device CreateDevice(Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override Adapter[] EnumerateAdapters()
    {
        uint pDeviceCount = 0;
        Vk.EnumeratePhysicalDevices(Instance, ref pDeviceCount, null);
        PhysicalDevice[] devices = new PhysicalDevice[pDeviceCount];
        Vk.EnumeratePhysicalDevices(Instance, ref pDeviceCount, ref devices[0]);

        Adapter[] adapters = new Adapter[pDeviceCount];

        for (uint i = 0; i < pDeviceCount; i++)
        {
            PhysicalDevice device = devices[i];

            Vk.GetPhysicalDeviceProperties(device, out PhysicalDeviceProperties deviceProperties);
            Vk.GetPhysicalDeviceMemoryProperties(device, out PhysicalDeviceMemoryProperties memoryProperties);

            AdapterType type = deviceProperties.DeviceType switch
            {
                PhysicalDeviceType.Other => AdapterType.Other,
                PhysicalDeviceType.IntegratedGpu => AdapterType.Integrated,
                PhysicalDeviceType.DiscreteGpu => AdapterType.Discrete,
                PhysicalDeviceType.VirtualGpu => AdapterType.Other,
                PhysicalDeviceType.Cpu => AdapterType.Software,
                _ => throw new ArgumentOutOfRangeException()
            };

            adapters[i] = new Adapter(i, new string((sbyte*) deviceProperties.DeviceName),
                memoryProperties.MemoryHeaps.Element0.Size, type);
        }

        return adapters;
    }

    public override void Dispose()
    {
        Vk.DestroyInstance(Instance, null);
        Vk.Dispose();
    }
}