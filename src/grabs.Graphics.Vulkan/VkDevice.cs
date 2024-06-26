global using VulkanDevice = Silk.NET.Vulkan.Device;

using System;
using System.Collections.Generic;
using grabs.Core;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public unsafe class VkDevice : Device
{
    private readonly Vk _vk;
    private readonly VulkanInstance _instance;
    
    private readonly KhrSwapchain _khrSwapchain;

    public readonly uint GraphicsFamily;
    public readonly uint PresentFamily;
    public readonly uint? ComputeFamily;

    public readonly VulkanDevice Device;
    
    public VkDevice(Vk vk, VulkanInstance instance, KhrSurface khrSurface, PhysicalDevice device, VkSurface surface)
    {
        _vk = vk;
        _instance = instance;

        uint? graphicsFamily = null;
        uint? presentFamily = null;
        uint? computeFamily = null;

        uint numFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(device, &numFamilies, null);
        Span<QueueFamilyProperties> familyProps = stackalloc QueueFamilyProperties[(int) numFamilies];
        fixed (QueueFamilyProperties* pProps = familyProps)
            _vk.GetPhysicalDeviceQueueFamilyProperties(device, &numFamilies, pProps);

        for (uint i = 0; i < numFamilies; i++)
        {
            QueueFamilyProperties props = familyProps[(int) i];

            if ((props.QueueFlags & QueueFlags.GraphicsBit) == QueueFlags.GraphicsBit)
                graphicsFamily = i;

            if ((props.QueueFlags & QueueFlags.ComputeBit) == QueueFlags.ComputeBit)
                computeFamily = i;

            khrSurface.GetPhysicalDeviceSurfaceSupport(device, i, surface.Surface, out Bool32 supported);
            if (supported)
                presentFamily = i;

            if (graphicsFamily.HasValue && presentFamily.HasValue && computeFamily.HasValue)
                break;
        }

        if (!graphicsFamily.HasValue || !presentFamily.HasValue)
        {
            throw new Exception(
                $"Given device did not support graphics or presentation: (Values: graphics: {graphicsFamily?.ToString() ?? "null"}, present: {presentFamily?.ToString() ?? "null"})");
        }

        GraphicsFamily = graphicsFamily.Value;
        PresentFamily = presentFamily.Value;
        ComputeFamily = computeFamily;

        using PinnedStringArray extensions = new PinnedStringArray(KhrSwapchain.ExtensionName);
        GrabsLog.Log(GrabsLog.LogType.Debug, $"extensions: {extensions}");

        HashSet<uint> uniqueQueueFamilies = new HashSet<uint>() { GraphicsFamily, PresentFamily };
        if (ComputeFamily.HasValue)
            uniqueQueueFamilies.Add(ComputeFamily.Value);

        int numQueues = uniqueQueueFamilies.Count;
        DeviceQueueCreateInfo* queueCreateInfos = stackalloc DeviceQueueCreateInfo[numQueues];

        float queuePriority = 1.0f;
        int qI = 0;
        foreach (uint queueFamily in uniqueQueueFamilies)
        {
            queueCreateInfos[qI++] = new DeviceQueueCreateInfo()
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = queueFamily,
                QueueCount = 1,
                PQueuePriorities = &queuePriority
            };
        }

        PhysicalDeviceFeatures features = new PhysicalDeviceFeatures();

        DeviceCreateInfo deviceCreateInfo = new DeviceCreateInfo()
        {
            SType = StructureType.DeviceCreateInfo,
            
            PpEnabledExtensionNames = extensions,
            EnabledExtensionCount = extensions.Length,
            
            QueueCreateInfoCount = (uint) numQueues,
            PQueueCreateInfos = queueCreateInfos,
            
            PEnabledFeatures = &features
        };

        GrabsLog.Log(GrabsLog.LogType.Debug, "Creating device.");
        
        Result result;
        if ((result = _vk.CreateDevice(device, &deviceCreateInfo, null, out Device)) != Result.Success)
            throw new Exception($"Failed to create device: {result}");
        
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Requesting KHRSwapchain extension.");
        if (!_vk.TryGetDeviceExtension(_instance, Device, out _khrSwapchain))
            throw new Exception("Failed to get KHRSwapchain extension.");
    }
    
    public override Swapchain CreateSwapchain(in SwapchainDescription description)
    {
        return new VkSwapchain(_vk, _khrSwapchain, Device, description);
    }

    public override CommandList CreateCommandList()
    {
        throw new NotImplementedException();
    }

    public override Pipeline CreatePipeline(in PipelineDescription description)
    {
        throw new NotImplementedException();
    }

    public override unsafe Buffer CreateBuffer(in BufferDescription description, void* pData)
    {
        throw new NotImplementedException();
    }

    public override unsafe Texture CreateTexture(in TextureDescription description, void** ppData)
    {
        throw new NotImplementedException();
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        throw new NotImplementedException();
    }

    public override Framebuffer CreateFramebuffer(in ReadOnlySpan<Texture> colorTextures, Texture depthTexture = null)
    {
        throw new NotImplementedException();
    }

    public override DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutDescription description)
    {
        throw new NotImplementedException();
    }

    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, in ReadOnlySpan<DescriptorSetDescription> descriptions)
    {
        throw new NotImplementedException();
    }

    public override void UpdateDescriptorSet(DescriptorSet set, in ReadOnlySpan<DescriptorSetDescription> descriptions)
    {
        throw new NotImplementedException();
    }

    public override IntPtr MapBuffer(Buffer buffer, MapMode mapMode)
    {
        throw new NotImplementedException();
    }

    public override void UnmapBuffer(Buffer buffer)
    {
        throw new NotImplementedException();
    }

    public override void ExecuteCommandList(CommandList list)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        GrabsLog.Log(GrabsLog.LogType.Verbose, "Destroying device.");
        _vk.DestroyDevice(Device, null);
    }
}