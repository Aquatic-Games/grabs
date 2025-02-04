global using VkDevice = Silk.NET.Vulkan.Device;
using grabs.Core;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;

    private readonly uint _graphicsQueueIndex;
    private readonly uint _presentQueueIndex;

    public readonly VkDevice Device;

    public readonly Queue GraphicsQueue;
    public readonly Queue PresentQueue;

    public VulkanDevice(Vk vk, VkInstance instance, PhysicalDevice physicalDevice, VulkanSurface surface, KhrSurface khrSurface)
    {
        _vk = vk;

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

        _graphicsQueueIndex = graphicsQueueIndex.Value;
        _presentQueueIndex = presentQueueIndex.Value;

        HashSet<uint> uniqueQueueFamilies = [_graphicsQueueIndex, _presentQueueIndex];

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

        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General, "Creating device.");
        _vk.CreateDevice(physicalDevice, &deviceInfo, null, out Device).Check("Create device");

        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General, "Get graphics queue");
        _vk.GetDeviceQueue(Device, _graphicsQueueIndex, 0, out GraphicsQueue);
        
        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General, "Get present queue");
        _vk.GetDeviceQueue(Device, _presentQueueIndex, 0, out PresentQueue);
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        _vk.DestroyDevice(Device, null);
    }
}