using System.Diagnostics;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkCommandList : CommandList
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly CommandPool _pool;

    private VkTexture? _currentSwapchainTexture;

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

    public override void Begin()
    {
        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
        };

        _vk.BeginCommandBuffer(CommandBuffer, &beginInfo).Check("Begin command buffer");
    }
    
    public override void End()
    {
        _vk.EndCommandBuffer(CommandBuffer).Check("End command buffer");
    }

    public override void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        Debug.Assert(colorAttachments.Length > 0);
        
        RenderingAttachmentInfo* colorRenderAttachments = stackalloc RenderingAttachmentInfo[colorAttachments.Length];

        for (int i = 0; i < colorAttachments.Length; i++)
        {
            ref readonly ColorAttachmentInfo attachment = ref colorAttachments[i];
            VkTexture texture = ((VkTexture) attachment.Texture);

            if (texture.IsSwapchainTexture)
            {
                _currentSwapchainTexture?.Transition(CommandBuffer, ImageLayout.ColorAttachmentOptimal,
                    ImageLayout.PresentSrcKhr);

                _currentSwapchainTexture = texture;
                _currentSwapchainTexture.Transition(CommandBuffer, ImageLayout.Undefined, ImageLayout.ColorAttachmentOptimal);
            }

            colorRenderAttachments[i] = new RenderingAttachmentInfo()
            {
                SType = StructureType.RenderingAttachmentInfo,
                ImageView = texture.ImageView,
                ImageLayout = ImageLayout.ColorAttachmentOptimal,
                ClearValue = new ClearValue()
                {
                    Color = new ClearColorValue(attachment.ClearColor.R, attachment.ClearColor.G,
                        attachment.ClearColor.B, attachment.ClearColor.A)
                },
                LoadOp = attachment.LoadOp.ToVk(),
                StoreOp = attachment.StoreOp.ToVk()
            };
        }

        Extent2D size = colorAttachments[0].Texture.Size.ToVk();

        RenderingInfo renderingInfo = new()
        {
            SType = StructureType.RenderingInfo,
            ColorAttachmentCount = (uint) colorAttachments.Length,
            PColorAttachments = colorRenderAttachments,
            LayerCount = 1,
            RenderArea = new Rect2D()
            {
                Extent = size
            }
        };
        
        _vk.CmdBeginRendering(CommandBuffer, &renderingInfo);
        
        Viewport vp = new()
        {
            X = 0,
            Y = size.Height,
            Width = size.Width,
            Height = -size.Height,
            MinDepth = 0,
            MaxDepth = 1
        };
        _vk.CmdSetViewport(CommandBuffer, 0, 1, &vp);

        Rect2D scissor = new()
        {
            Extent = size
        };
        _vk.CmdSetScissor(CommandBuffer, 0, 1, &scissor);
    }
    
    public override void EndRenderPass()
    {
        _vk.CmdEndRendering(CommandBuffer);
        
        _currentSwapchainTexture?.Transition(CommandBuffer, ImageLayout.ColorAttachmentOptimal, ImageLayout.PresentSrcKhr);
        _currentSwapchainTexture = null;
    }

    public override void SetGraphicsPipeline(Pipeline pipeline)
    {
        VkPipeline vkPipeline = (VkPipeline) pipeline;
        _vk.CmdBindPipeline(CommandBuffer, PipelineBindPoint.Graphics, vkPipeline.Pipeline);
    }
    
    public override void Draw(uint numVertices)
    {
        _vk.CmdDraw(CommandBuffer, numVertices, 1, 0, 0);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.FreeCommandBuffers(_device, _pool, 1, in CommandBuffer);
    }
}