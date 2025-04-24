global using VulkanDevice = Silk.NET.Vulkan.Device;
using grabs.Core;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkDevice : Device
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VulkanInstance _instance;

    public readonly VulkanDevice Device;
    public readonly Queues Queues;
    
    public VkDevice(Vk vk, VulkanInstance instance, KhrSurface khrSurface, PhysicalDevice physicalDevice, SurfaceKHR surface)
    {
        ResourceManager.RegisterInstanceResource(instance, this);
        
        _vk = vk;
        _instance = instance;

        uint? graphicsQueue = null;
        uint? presentQueue = null;

        uint numFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &numFamilies, null);
        QueueFamilyProperties* queueFamilies = stackalloc QueueFamilyProperties[(int) numFamilies];
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &numFamilies, queueFamilies);

        for (uint i = 0; i < numFamilies; i++)
        {
            if ((queueFamilies[i].QueueFlags & QueueFlags.GraphicsBit) == QueueFlags.GraphicsBit)
                graphicsQueue = i;

            khrSurface.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, surface, out Bool32 supported)
                .Check("Check physical device surface support");

            if (supported)
                presentQueue = i;

            if (graphicsQueue.HasValue && presentQueue.HasValue)
                break;
        }

        if (!graphicsQueue.HasValue || !presentQueue.HasValue)
        {
            throw new Exception(
                $"Graphics/Present queue(s) not found. Graphics: {graphicsQueue.HasValue}, Present: {graphicsQueue.HasValue}");
        }

        Queues.GraphicsIndex = graphicsQueue.Value;
        Queues.PresentIndex = presentQueue.Value;

        HashSet<uint> uniqueQueues = Queues.UniqueQueues;

        DeviceQueueCreateInfo* queueInfos = stackalloc DeviceQueueCreateInfo[uniqueQueues.Count];

        int index = 0;
        float priority = 1.0f;
        
        foreach (uint family in uniqueQueues)
        {
            queueInfos[index++] = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueCount = 1,
                QueueFamilyIndex = family,
                PQueuePriorities = &priority
            };
        }

        PhysicalDeviceFeatures features = new();

        using Utf8Array pExtensions = new(KhrSwapchain.ExtensionName);
        
        DeviceCreateInfo deviceInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,
            
            QueueCreateInfoCount = (uint) uniqueQueues.Count,
            PQueueCreateInfos = queueInfos,
            
            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions,
            
            PEnabledFeatures = &features
        };
        
        GrabsLog.Log("Creating device.");
        _vk.CreateDevice(physicalDevice, &deviceInfo, null, out Device).Check("Create device");
        
        GrabsLog.Log("Getting queues.");
        _vk.GetDeviceQueue(Device, Queues.GraphicsIndex, 0, out Queues.Graphics);
        _vk.GetDeviceQueue(Device, Queues.PresentIndex, 0, out Queues.Present);
    }
    
    public override void Dispose()
    {
        GrabsLog.Log("Destroying device.");
        _vk.DestroyDevice(Device, null);
        
        ResourceManager.DeregisterInstanceResource(_instance, this);
    }
}