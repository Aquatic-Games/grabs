using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkCommandList : CommandList
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly CommandPool _pool;

    public readonly CommandBuffer CommandBuffer;

    public VkCommandList(Vk vk, VulkanDevice device, CommandPool pool)
    {
        _vk = vk;
        _device = device;
        _pool = pool;
        
        CommandBufferAllocateInfo cbAllocInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _pool,
            CommandBufferCount = 1,
            Level = CommandBufferLevel.Primary
        };

        _vk.AllocateCommandBuffers(_device, &cbAllocInfo, out CommandBuffer).Check("Allocate command buffer");
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.FreeCommandBuffers(_device, _pool, 1, in CommandBuffer);
    }
}