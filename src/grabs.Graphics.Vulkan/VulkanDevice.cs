global using VkDevice = Silk.NET.Vulkan.Device;
using System.Diagnostics;
using System.Runtime.InteropServices;
using grabs.Core;
using grabs.VulkanMemoryAllocator;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanDevice : Device
{
    private readonly Vk _vk;

    private GCHandle _instanceFnHandle;
    private GCHandle _deviceFnHandle;
    
    public readonly PhysicalDevice PhysicalDevice;
    
    public readonly KhrSurface KhrSurface;
    
    public readonly KhrSwapchain KhrSwapchain;
    
    public readonly VkDevice Device;

    public readonly Queues Queues;

    public readonly CommandPool CommandPool;
    public readonly CommandBuffer DeviceCommandBuffer;

    public readonly VmaAllocator_T* Allocator;

    private readonly Fence _fence;
    
    public override Adapter Adapter { get; }

    public VulkanDevice(Vk vk, VkInstance instance, in Adapter adapter, VulkanSurface surface, KhrSurface khrSurface)
    {
        Debug.Assert(adapter.Handle != 0);
        
        _vk = vk;
        Adapter = adapter;
        PhysicalDevice = new PhysicalDevice(adapter.Handle);
        KhrSurface = khrSurface;

        uint? graphicsQueueIndex = null;
        uint? presentQueueIndex = null;

        uint numQueueFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &numQueueFamilies, null);
        QueueFamilyProperties* queueFamilies = stackalloc QueueFamilyProperties[(int) numQueueFamilies];
        _vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, &numQueueFamilies, queueFamilies);

        for (uint i = 0; i < numQueueFamilies; i++)
        {
            if (queueFamilies[i].QueueFlags.HasFlag(QueueFlags.TransferBit))
                graphicsQueueIndex = i;

            khrSurface.GetPhysicalDeviceSurfaceSupport(PhysicalDevice, i, surface.Surface, out Bool32 supported);

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
        _vk.CreateDevice(PhysicalDevice, &deviceInfo, null, out Device).Check("Create device");

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

        CommandBufferAllocateInfo commandBufferInfo = new CommandBufferAllocateInfo()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = CommandPool,
            CommandBufferCount = 1,
            Level = CommandBufferLevel.Primary
        };
        
        GrabsLog.Log("Creating device command buffer.");
        _vk.AllocateCommandBuffers(Device, &commandBufferInfo, out DeviceCommandBuffer)
            .Check("Allocate device command buffer");
        
        FenceCreateInfo fenceInfo = new FenceCreateInfo()
        {
            SType = StructureType.FenceCreateInfo
        };

        GrabsLog.Log("Creating fence");
        _vk.CreateFence(Device, &fenceInfo, null, out _fence).Check("Create fence");

        _instanceFnHandle = GCHandle.Alloc(GetInstanceProcAddress);
        _deviceFnHandle = GCHandle.Alloc(GetDeviceProcAddress);
        
        VmaVulkanFunctions functions = new VmaVulkanFunctions();
        functions.vkGetInstanceProcAddr = (delegate* unmanaged[Cdecl]<VkInstance, sbyte*, delegate* unmanaged[Cdecl]<void>>) Marshal.GetFunctionPointerForDelegate(GetInstanceProcAddress);
        functions.vkGetDeviceProcAddr = (delegate* unmanaged[Cdecl]<VkDevice, sbyte*, delegate* unmanaged[Cdecl]<void>>) Marshal.GetFunctionPointerForDelegate(GetDeviceProcAddress);

        VmaAllocatorCreateInfo allocatorInfo = new VmaAllocatorCreateInfo()
        {
            instance = instance,
            physicalDevice = PhysicalDevice,
            device = Device,
            vulkanApiVersion = Vk.MakeVersion(1, 3),
            pVulkanFunctions = &functions
        };

        GrabsLog.Log("Creating allocator");
        Vma.CreateAllocator(&allocatorInfo, out Allocator).Check("Create allocator");
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new VulkanSwapchain(_vk, this, in info);
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

    public override Buffer CreateBuffer(in BufferInfo info, void* data)
    {
        return new VulkanBuffer(_vk, this, in info, data);
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
    
    public CommandBuffer BeginCommands()
    {
        CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo()
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit
        };
        
        _vk.BeginCommandBuffer(DeviceCommandBuffer, &beginInfo).Check("Begin one time command buffer");

        return DeviceCommandBuffer;
    }

    public void EndCommands()
    {
        CommandBuffer cb = DeviceCommandBuffer;
        
        _vk.EndCommandBuffer(cb).Check("End one time command buffer");

        SubmitInfo submitInfo = new SubmitInfo()
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &cb
        };

        _vk.QueueSubmit(Queues.Graphics, 1, &submitInfo, new Fence()).Check("Submit one time queue");
        _vk.QueueWaitIdle(Queues.Graphics).Check("Wait for queue idle");
    }

    public override void Dispose()
    {
        Vma.DestroyAllocator(Allocator);
        _deviceFnHandle.Free();
        _instanceFnHandle.Free();
        
        _vk.DestroyFence(Device, _fence, null);
        
        _vk.DestroyCommandPool(Device, CommandPool, null);
        
        _vk.DestroyDevice(Device, null);
    }

    private void* GetInstanceProcAddress(VkInstance instance, byte* name)
        => _vk.GetInstanceProcAddr(instance, name);

    private void* GetDeviceProcAddress(VkDevice device, byte* name)
        => _vk.GetDeviceProcAddr(device, name);
}