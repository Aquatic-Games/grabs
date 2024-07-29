global using VulkanDevice = Silk.NET.Vulkan.Device;

using System;
using System.Collections.Generic;
using grabs.Core;
using Silk.NET.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using static grabs.Graphics.Vulkan.VkResult;

namespace grabs.Graphics.Vulkan;

public sealed unsafe class VkDevice : Device
{
    private readonly Vk _vk;
    
    private readonly uint _graphicsQueueIndex;
    private readonly uint _presentQueueIndex;
    
    public readonly VulkanDevice Device;

    public readonly KhrSwapchain KhrSwapchain;

    public readonly Queue GraphicsQueue;
    public readonly Queue PresentQueue;

    public VkDevice(Vk vk, VulkanInstance instance, PhysicalDevice pDevice, VkSurface surface)
    {
        _vk = vk;

        uint numQueueFamilies;
        _vk.GetPhysicalDeviceQueueFamilyProperties(pDevice, &numQueueFamilies, null);
        QueueFamilyProperties* queueFamilies = stackalloc QueueFamilyProperties[(int) numQueueFamilies];
        _vk.GetPhysicalDeviceQueueFamilyProperties(pDevice, &numQueueFamilies, queueFamilies);

        uint? graphicsQueue = null;
        uint? presentQueue = null;
        
        for (uint i = 0; i < numQueueFamilies; i++)
        {
            QueueFamilyProperties* family = &queueFamilies[i];

            if ((family->QueueFlags & QueueFlags.GraphicsBit) == QueueFlags.GraphicsBit)
                graphicsQueue = i;

            surface.KhrSurface.GetPhysicalDeviceSurfaceSupport(pDevice, i, surface.Surface, out Bool32 supported);
            if (supported)
                presentQueue = i;

            if (graphicsQueue.HasValue && presentQueue.HasValue)
                break;
        }

        if (!graphicsQueue.HasValue || !presentQueue.HasValue)
        {
            throw new Exception(
                $"Cannot create device: Graphics or Present queue not available. (GQueue: {graphicsQueue}, PQueue: {presentQueue})");
        }

        _graphicsQueueIndex = graphicsQueue.Value;
        _presentQueueIndex = presentQueue.Value;

        HashSet<uint> uniqueQueueFamilies = [_graphicsQueueIndex, _presentQueueIndex];
        DeviceQueueCreateInfo* queueCreateInfos = stackalloc DeviceQueueCreateInfo[uniqueQueueFamilies.Count];

        int currentQueueIndex = 0;
        float queuePriority = 1.0f;
        foreach (uint queue in uniqueQueueFamilies)
        {
            queueCreateInfos[currentQueueIndex++] = new DeviceQueueCreateInfo()
            {
                SType = StructureType.DeviceQueueCreateInfo,

                QueueCount = 1,
                QueueFamilyIndex = queue,
                PQueuePriorities = &queuePriority
            };
        }

        using PinnedStringArray deviceExtensions = new PinnedStringArray(KhrSwapchain.ExtensionName);

        PhysicalDeviceFeatures enabledFeatures = new PhysicalDeviceFeatures();

        DeviceCreateInfo deviceCreateInfo = new DeviceCreateInfo()
        {
            SType = StructureType.DeviceCreateInfo,
            
            QueueCreateInfoCount = (uint) uniqueQueueFamilies.Count,
            PQueueCreateInfos = queueCreateInfos,
            
            EnabledExtensionCount = deviceExtensions.Length,
            PpEnabledExtensionNames = deviceExtensions,
            
            PEnabledFeatures = &enabledFeatures
        };

        CheckResult(_vk.CreateDevice(pDevice, &deviceCreateInfo, null, out Device), "Create device");

        if (!_vk.TryGetDeviceExtension(instance, Device, out KhrSwapchain))
            throw new Exception("Failed to get VK_KHR_swapchain device extension.");

        _vk.GetDeviceQueue(Device, _graphicsQueueIndex, 0, out GraphicsQueue);
        _vk.GetDeviceQueue(Device, _presentQueueIndex, 0, out PresentQueue);
    }
    
    public override Swapchain CreateSwapchain(in SwapchainDescription description)
    {
        throw new NotImplementedException();
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

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint,
        SpecializationConstant[] constants = null)
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

    public override Sampler CreateSampler(in SamplerDescription description)
    {
        throw new NotImplementedException();
    }

    public override unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData)
    {
        throw new NotImplementedException();
    }

    public override unsafe void UpdateTexture(Texture texture, int x, int y, uint width, uint height, uint mipLevel, void* pData)
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
        CheckResult(_vk.DeviceWaitIdle(Device));
        KhrSwapchain.Dispose();
        _vk.DestroyDevice(Device, null);
    }
}