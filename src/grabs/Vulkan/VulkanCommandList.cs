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

    public override void BeginRenderPass(in RenderPassInfo info)
    {
        RenderingAttachmentInfo* colorAttachments = stackalloc RenderingAttachmentInfo[info.ColorAttachments.Length];

        for (int i = 0; i < info.ColorAttachments.Length; i++)
        {
            ref ColorAttachmentInfo attachmentInfo = ref info.ColorAttachments[i];
            ColorF clearColor = attachmentInfo.ClearColor;

            VulkanTexture texture = (VulkanTexture) attachmentInfo.Texture;

            colorAttachments[i] = new RenderingAttachmentInfo()
            {
                SType = StructureType.RenderingAttachmentInfo,
                ImageView = texture.ImageView,
                ImageLayout = ImageLayout.ColorAttachmentOptimal,
                ClearValue = new ClearValue(new ClearColorValue(clearColor.R, clearColor.G, clearColor.B, clearColor.A)),
                
                LoadOp = AttachmentLoadOp.Clear,
                StoreOp = AttachmentStoreOp.None
            };
        }
        
        RenderingInfo renderingInfo = new RenderingInfo()
        {
            SType = StructureType.RenderingInfo,

            LayerCount = 1,
            RenderArea = new Rect2D(extent: new Extent2D(1280, 720)),
            
            ColorAttachmentCount = (uint) info.ColorAttachments.Length,
            PColorAttachments = colorAttachments
        };
        
        _vk.CmdBeginRendering(Buffer, &renderingInfo);
    }
    public override void EndRenderPass()
    {
        _vk.CmdEndRendering(Buffer);
    }

    public override void Dispose()
    {
        fixed (CommandBuffer* buffer = &Buffer)
            _vk.FreeCommandBuffers(_device, _pool, 1, buffer);
    }
}