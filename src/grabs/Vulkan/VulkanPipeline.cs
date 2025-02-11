global using VkPipeline = Silk.NET.Vulkan.Pipeline;
using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal sealed class VulkanPipeline : Pipeline
{
    private readonly Vk _vk;
    
    public readonly VkPipeline Pipeline;
    
    public VulkanPipeline(Vk vk, in PipelineInfo pipelineInfo)
    {
        _vk = vk;
        
        
    }
    
    public override void Dispose()
    {
        
    }
}