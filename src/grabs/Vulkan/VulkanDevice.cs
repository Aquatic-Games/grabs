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
    
    public readonly PhysicalDevice PhysicalDevice;
    
    public readonly KhrSurface KhrSurface;
    
    public readonly KhrSwapchain KhrSwapchain;
    
    public readonly VkDevice Device;

    public readonly Queues Queues;

    public readonly CommandPool CommandPool;

    private readonly Fence _fence;

    public VulkanDevice(Vk vk, VkInstance instance, PhysicalDevice physicalDevice, VulkanSurface surface, KhrSurface khrSurface)
    {
        _vk = vk;
        PhysicalDevice = physicalDevice;
        KhrSurface = khrSurface;

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

        // We must manually enable dynamic rendering.
        PhysicalDeviceDynamicRenderingFeatures dynamicRenderingFeatures = new PhysicalDeviceDynamicRenderingFeatures()
        {
            SType = StructureType.PhysicalDeviceDynamicRenderingFeatures,
            DynamicRendering = true
        };
        
        deviceInfo.PNext = &dynamicRenderingFeatures;

        GrabsLog.Log("Creating device.");
        _vk.CreateDevice(physicalDevice, &deviceInfo, null, out Device).Check("Create device");

        GrabsLog.Log("Get graphics queue");
        _vk.GetDeviceQueue(Device, Queues.GraphicsIndex, 0, out Queues.Graphics);
        
        GrabsLog.Log("Get present queue");
        _vk.GetDeviceQueue(Device, Queues.PresentIndex, 0, out Queues.Present);

        if (!_vk.TryGetDeviceExtension(instance, Device, out KhrSwapchain))
            throw new Exception("Failed to get Swapchain extension.");

        CommandPoolCreateInfo commandPoolInfo = new CommandPoolCreateInfo()
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = Queues.GraphicsIndex,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit
        };
        
        GrabsLog.Log("Creating command pool");
        _vk.CreateCommandPool(Device, &commandPoolInfo, null, out CommandPool).Check("Create command pool");

        FenceCreateInfo fenceInfo = new FenceCreateInfo()
        {
            SType = StructureType.FenceCreateInfo
        };

        GrabsLog.Log("Creating fence");
        _vk.CreateFence(Device, &fenceInfo, null, out _fence).Check("Create fence");
    }

    public override Swapchain CreateSwapchain(Surface surface, in SwapchainInfo info)
    {
        return new VulkanSwapchain(_vk, this, (VulkanSurface) surface, in info);
    }

    public override CommandList CreateCommandList()
    {
        return new VulkanCommandList(_vk, Device, CommandPool);
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        ReadOnlySpan<byte> spirvSpan = spirv.AsSpan();
        
        return new VulkanShaderModule(_vk, Device, stage, ref spirvSpan, entryPoint);
    }

    public override Pipeline CreatePipeline(in PipelineInfo info)
    {
        return new VulkanPipeline(_vk, Device, in info);
    }

    public override void ExecuteCommandList(CommandList list)
    {
        VulkanCommandList vkList = (VulkanCommandList) list;
        CommandBuffer buffer = vkList.Buffer;

        SubmitInfo submitInfo = new SubmitInfo()
        {
            SType = StructureType.SubmitInfo,

            CommandBufferCount = 1,
            PCommandBuffers = &buffer,
        };

        // TODO: It goes without saying that relying on a fence for sync is bad and slow.
        //       This should use semaphores instead once things are figured out.
        _vk.QueueSubmit(Queues.Graphics, 1, &submitInfo, _fence).Check("Submit queue");

        _vk.WaitForFences(Device, 1, in _fence, true, ulong.MaxValue);
        _vk.ResetFences(Device, 1, in _fence);
    }

    public override void WaitForIdle()
    {
        _vk.DeviceWaitIdle(Device).Check("Wait for idle");
    }

    public override void Dispose()
    {
        _vk.DestroyFence(Device, _fence, null);
        
        _vk.DestroyCommandPool(Device, CommandPool, null);
        
        _vk.DestroyDevice(Device, null);
    }
}