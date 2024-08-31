using System.Drawing;
using Silk.NET.Vulkan;
using static grabs.Graphics.Vulkan.VkResult;

namespace grabs.Graphics.Vulkan;

public sealed unsafe class VkCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly CommandPool _commandPool;
    
    public readonly CommandBuffer CommandBuffer;

    public VkCommandList(Vk vk, VulkanDevice device, CommandPool pool)
    {
        _vk = vk;
        _device = device;
        _commandPool = pool;
        
        CommandBufferAllocateInfo commandBufferInfo = new CommandBufferAllocateInfo()
        {
            SType = StructureType.CommandBufferAllocateInfo,

            CommandPool = pool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = 1
        };
        
        CheckResult(_vk.AllocateCommandBuffers(device, &commandBufferInfo, out CommandBuffer));
    }
    
    public override void Begin()
    {
        throw new System.NotImplementedException();
    }

    public override void End()
    {
        throw new System.NotImplementedException();
    }

    public override void BeginRenderPass(in RenderPassDescription description)
    {
        throw new System.NotImplementedException();
    }

    public override void EndRenderPass()
    {
        throw new System.NotImplementedException();
    }

    public override unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData)
    {
        throw new System.NotImplementedException();
    }

    public override void GenerateMipmaps(Texture texture)
    {
        throw new System.NotImplementedException();
    }

    public override void SetViewport(in Viewport viewport)
    {
        throw new System.NotImplementedException();
    }

    public override void SetScissor(in Rectangle rectangle)
    {
        throw new System.NotImplementedException();
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        throw new System.NotImplementedException();
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset)
    {
        throw new System.NotImplementedException();
    }

    public override void SetIndexBuffer(Buffer buffer, Format format)
    {
        throw new System.NotImplementedException();
    }

    public override void SetDescriptorSet(uint index, DescriptorSet set)
    {
        throw new System.NotImplementedException();
    }

    public override void Draw(uint numVertices)
    {
        throw new System.NotImplementedException();
    }

    public override void DrawIndexed(uint numIndices)
    {
        throw new System.NotImplementedException();
    }

    public override void DrawIndexed(uint numIndices, uint startIndex, int baseVertex)
    {
        throw new System.NotImplementedException();
    }

    public override void Dispose()
    {
        _vk.FreeCommandBuffers(_device, _commandPool, 1, in CommandBuffer);
    }
}