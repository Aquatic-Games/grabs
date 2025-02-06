global using VkDevice = Silk.NET.Vulkan.Device;
using grabs.Core;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;
    private readonly PhysicalDevice _physicalDevice;
    private readonly KhrSurface _khrSurface;
    private readonly KhrSwapchain _khrSwapchain;
    
    public readonly VkDevice Device;

    public readonly Queues Queues;

    public readonly CommandPool CommandPool;

    // TODO: Better names
    private bool _alternateSemaphore;
    public readonly Semaphore Semaphore1;
    public readonly Semaphore Semaphore2;

    public Semaphore HeadSemaphore => _alternateSemaphore ? Semaphore2 : Semaphore1;

    public VulkanDevice(Vk vk, VkInstance instance, PhysicalDevice physicalDevice, VulkanSurface surface, KhrSurface khrSurface)
    {
        _vk = vk;
        _physicalDevice = physicalDevice;
        _khrSurface = khrSurface;

        uint? graphicsQueueIndex = null;
        uint? presentQueueIndex = null;

        uint numQueueFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &numQueueFamilies, null);
        QueueFamilyProperties* queueFamilies = stackalloc QueueFamilyProperties[(int) numQueueFamilies];
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &numQueueFamilies, queueFamilies);

        for (uint i = 0; i < numQueueFamilies; i++)
        {
            if (queueFamilies[i].QueueFlags.HasFlag(QueueFlags.TransferBit))
                graphicsQueueIndex = i;

            khrSurface.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, surface.Surface, out Bool32 supported);

            if (supported)
                presentQueueIndex = i;

            if (graphicsQueueIndex.HasValue && presentQueueIndex.HasValue)
                break;
        }

        if (!graphicsQueueIndex.HasValue || !presentQueueIndex.HasValue)
            throw new Exception("Graphics/present queue not found.");

        Queues.GraphicsIndex = graphicsQueueIndex.Value;
        Queues.PresentIndex = presentQueueIndex.Value;

        HashSet<uint> uniqueQueueFamilies = Queues.UniqueQueues;

        DeviceQueueCreateInfo* queueInfos = stackalloc DeviceQueueCreateInfo[uniqueQueueFamilies.Count];

        int familyIndex = 0;
        foreach (uint family in uniqueQueueFamilies)
        {
            float queuePriority = 1.0f;

            queueInfos[familyIndex] = new DeviceQueueCreateInfo()
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueCount = 1,
                QueueFamilyIndex = family,
                PQueuePriorities = &queuePriority
            };
        }

        PhysicalDeviceFeatures enabledFeatures = new PhysicalDeviceFeatures();

        using PinnedStringArray extensions = new PinnedStringArray(KhrSwapchain.ExtensionName);

        DeviceCreateInfo deviceInfo = new DeviceCreateInfo()
        {
            SType = StructureType.DeviceCreateInfo,

            PQueueCreateInfos = queueInfos,
            QueueCreateInfoCount = (uint) uniqueQueueFamilies.Count,

            PEnabledFeatures = &enabledFeatures,

            PpEnabledExtensionNames = extensions,
            EnabledExtensionCount = extensions.Length
        };

        GrabsLog.Log("Creating device.");
        _vk.CreateDevice(physicalDevice, &deviceInfo, null, out Device).Check("Create device");

        GrabsLog.Log("Get graphics queue");
        _vk.GetDeviceQueue(Device, Queues.GraphicsIndex, 0, out Queues.Graphics);
        
        GrabsLog.Log("Get present queue");
        _vk.GetDeviceQueue(Device, Queues.PresentIndex, 0, out Queues.Present);

        if (!_vk.TryGetDeviceExtension(instance, Device, out _khrSwapchain))
            throw new Exception("Failed to get Swapchain extension.");

        CommandPoolCreateInfo commandPoolInfo = new CommandPoolCreateInfo()
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = Queues.GraphicsIndex,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit
        };
        
        GrabsLog.Log("Creating command pool");
        _vk.CreateCommandPool(Device, &commandPoolInfo, null, out CommandPool).Check("Create command pool");
        
        SemaphoreCreateInfo semaphoreInfo = new SemaphoreCreateInfo()
        {
            SType = StructureType.SemaphoreCreateInfo,
        };
        
        GrabsLog.Log("Creating semaphores");
        _vk.CreateSemaphore(Device, &semaphoreInfo, null, out Semaphore1).Check("Create semaphore");
        _vk.CreateSemaphore(Device, &semaphoreInfo, null, out Semaphore2).Check("Create semaphore");
    }

    public override Swapchain CreateSwapchain(Surface surface, in SwapchainInfo info)
    {
        return new VulkanSwapchain(_khrSwapchain, _physicalDevice, Device, _khrSurface, in Queues,
            (VulkanSurface) surface, in info);
    }

    public override void Dispose()
    {
        _vk.DestroySemaphore(Device, Semaphore2, null);
        _vk.DestroySemaphore(Device, Semaphore1, null);
        
        _vk.DestroyCommandPool(Device, CommandPool, null);
        
        _vk.DestroyDevice(Device, null);
    }
}