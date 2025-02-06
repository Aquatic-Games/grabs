using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly CommandPool _pool;
    
    public readonly CommandBuffer Buffer;

    public VulkanCommandList(Vk vk, VkDevice device, CommandPool pool)
    {
        _vk = vk;
        _device = device;
        _pool = pool;
        
        CommandBufferAllocateInfo allocInfo = new CommandBufferAllocateInfo()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = pool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = 1
        };
        
        GrabsLog.Log("Allocating command buffer");
        _vk.AllocateCommandBuffers(device, &allocInfo, out Buffer).Check("Allocate command buffer");
    }

    public override void Begin()
    {
        CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo()
        {
            SType = StructureType.CommandBufferBeginInfo
        };
        
        _vk.BeginCommandBuffer(Buffer, &beginInfo).Check("Begin command buffer");
    }

    public override void End()
    {
        _vk.EndCommandBuffer(Buffer).Check("End command buffer");
    }

    public override void Dispose()
    {
        fixed (CommandBuffer* buffer = &Buffer)
            _vk.FreeCommandBuffers(_device, _pool, 1, buffer);
    }
}