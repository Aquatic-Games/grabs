global using VulkanPipeline = Silk.NET.Vulkan.Pipeline;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed class VkPipeline : Pipeline
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VulkanDevice _device;

    public readonly PipelineLayout Layout;
    public readonly VulkanPipeline Pipeline;

    public VkPipeline(Vk vk, VulkanDevice device, ref readonly GraphicsPipelineInfo info)
    {
        ResourceTracker.RegisterDeviceResource(device, this);

        _vk = vk;
        _device = device;
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        ResourceTracker.DeregisterDeviceResource(_device, this);
    }
}