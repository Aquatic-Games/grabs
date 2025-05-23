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

    private readonly CommandPool _commandPool;

    public readonly PhysicalDevice PhysicalDevice;
    
    public readonly VulkanDevice Device;
    public readonly Queues Queues;

    public readonly KhrSurface KhrSurface;
    public readonly KhrSwapchain KhrSwapchain;
    
    public override ShaderFormat ShaderFormat => ShaderFormat.Spirv;
    
    public VkDevice(Vk vk, VulkanInstance instance, KhrSurface khrSurface, PhysicalDevice physicalDevice, SurfaceKHR surface)
    {
        ResourceTracker.RegisterInstanceResource(instance, this);
        
        _vk = vk;
        _instance = instance;

        PhysicalDevice = physicalDevice;
        KhrSurface = khrSurface;

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

        PhysicalDeviceDynamicRenderingFeatures dynamicRendering = new()
        {
            SType = StructureType.PhysicalDeviceDynamicRenderingFeatures,
            DynamicRendering = true
        };
        
        DeviceCreateInfo deviceInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,
            PNext = &dynamicRendering,
            
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

        if (!_vk.TryGetDeviceExtension(_instance, Device, out KhrSwapchain))
            throw new Exception("Failed to get swapchain extension.");

        CommandPoolCreateInfo commandPoolInfo = new()
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = Queues.GraphicsIndex,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit
        };
        
        GrabsLog.Log("Creating command pool");
        _vk.CreateCommandPool(Device, &commandPoolInfo, null, out _commandPool).Check("Create command pool");
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new VkSwapchain(_vk, this, in info);
    }

    public override CommandList CreateCommandList()
    {
        return new VkCommandList(_vk, Device, _commandPool);
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] code, string entryPoint)
    {
        return new VkShaderModule(_vk, Device, code, entryPoint);
    }

    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        return new VkPipeline(_vk, Device, in info);
    }

    public override Buffer CreateBuffer(in BufferInfo info, void* pData)
    {
        throw new NotImplementedException();
    }

    public override void ExecuteCommandList(CommandList cl)
    {
        CommandBuffer buffer = ((VkCommandList) cl).CommandBuffer;

        SubmitInfo submitInfo = new()
        {
            SType = StructureType.SubmitInfo,

            CommandBufferCount = 1,
            PCommandBuffers = &buffer,
        };

        // TODO: Semaphores
        //   This is horribly inefficient and is only used temporarily during development.
        _vk.QueueSubmit(Queues.Graphics, 1, &submitInfo, new Fence()).Check("Submit queue");
        _vk.QueueWaitIdle(Queues.Graphics).Check("Wait for queue idle");
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        ResourceTracker.DisposeAllDeviceResources(Device);
        
        GrabsLog.Log("Destroying command pool.");
        _vk.DestroyCommandPool(Device, _commandPool, null);
        
        GrabsLog.Log("Destroying device.");
        _vk.DestroyDevice(Device, null);
        
        ResourceTracker.DeregisterInstanceResource(_instance, this);
    }
}