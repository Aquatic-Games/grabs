using System.Diagnostics;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

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
        Debug.Assert(info.ColorAttachments.Length > 0, "Render pass must have at least one color attachment.");
        
        RenderingAttachmentInfo* colorAttachments = stackalloc RenderingAttachmentInfo[info.ColorAttachments.Length];

        for (int i = 0; i < info.ColorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachmentInfo = ref info.ColorAttachments[i];
            ColorF clearColor = attachmentInfo.ClearColor;

            VulkanTexture texture = (VulkanTexture) attachmentInfo.Texture;

            colorAttachments[i] = new RenderingAttachmentInfo()
            {
                SType = StructureType.RenderingAttachmentInfo,
                ImageView = texture.ImageView,
                ImageLayout = ImageLayout.ColorAttachmentOptimal,
                ClearValue = new ClearValue(new ClearColorValue(clearColor.R, clearColor.G, clearColor.B, clearColor.A)),
                
                LoadOp = attachmentInfo.LoadOp.ToVk(),
                StoreOp = AttachmentStoreOp.Store
            };
        }
        
        RenderingInfo renderingInfo = new RenderingInfo()
        {
            SType = StructureType.RenderingInfo,

            LayerCount = 1,
            RenderArea = new Rect2D { Extent = info.ColorAttachments[0].Texture.Size.ToVk() },
            
            ColorAttachmentCount = (uint) info.ColorAttachments.Length,
            PColorAttachments = colorAttachments
        };
        
        _vk.CmdBeginRendering(Buffer, &renderingInfo);
    }
    public override void EndRenderPass()
    {
        _vk.CmdEndRendering(Buffer);
    }

    public override void SetViewport(in Viewport viewport)
    {
        Silk.NET.Vulkan.Viewport vkViewport = new Silk.NET.Vulkan.Viewport()
        {
            X = viewport.X,
            Y = viewport.Height,
            Width = viewport.Width,
            Height = -viewport.Height,
            MinDepth = viewport.MinDepth,
            MaxDepth = viewport.MaxDepth
        };
        
        _vk.CmdSetViewport(Buffer, 0, 1, &vkViewport);

        Rect2D scissor = new Rect2D(new Offset2D(0, 0), new Extent2D((uint) viewport.Width, (uint) viewport.Height));
        _vk.CmdSetScissor(Buffer, 0, 1, &scissor);
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        VulkanPipeline vkPipeline = (VulkanPipeline) pipeline;
        
        _vk.CmdBindPipeline(Buffer, PipelineBindPoint.Graphics, vkPipeline.Pipeline);
    }

    public override void Draw(uint numVertices)
    {
        _vk.CmdDraw(Buffer, numVertices, 1, 0, 0);
    }

    public override void Dispose()
    {
        fixed (CommandBuffer* buffer = &Buffer)
            _vk.FreeCommandBuffers(_device, _pool, 1, buffer);
    }
}