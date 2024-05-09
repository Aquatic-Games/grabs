using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public unsafe class VkDevice : Device
{
    private Vk _vk;

    public KhrSurface KhrSurface;

    public uint GraphicsFamily;
    public uint PresentFamily;
    public uint? ComputeFamily;
    
    public VkDevice(Vk vk, Silk.NET.Vulkan.Instance instance, PhysicalDevice device)
    {
        _vk = vk;

        if (!_vk.TryGetInstanceExtension(instance, out KhrSurface))
            throw new Exception("Could not get VK_KHR_surface extension. Is it supported?");

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

            if (graphicsFamily.HasValue && presentFamily.HasValue && computeFamily.HasValue)
                break;
        }

        if (!graphicsFamily.HasValue || !presentFamily.HasValue)
        {
            throw new Exception(
                $"Given device did not support graphics or presentation: (Values: graphics: {graphicsFamily}, present: {presentFamily})");
        }

        GraphicsFamily = graphicsFamily.Value;
        PresentFamily = presentFamily.Value;
        ComputeFamily = computeFamily;

        DeviceCreateInfo deviceCreateInfo = new DeviceCreateInfo()
        {
            SType = StructureType.DeviceCreateInfo
        };

        throw new NotImplementedException();
    }
    
    public override Swapchain CreateSwapchain(Surface surface, in SwapchainDescription description)
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

    public override unsafe Texture CreateTexture(in TextureDescription description, void* pData)
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

    public override void ExecuteCommandList(CommandList list)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}