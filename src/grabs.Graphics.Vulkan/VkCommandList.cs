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

    public override void BeginRenderPass(in ReadOnlySpan<ColorTargetInfo> colorTargets)
    {
        Debug.Assert(colorTargets.Length > 0);
        
        RenderingAttachmentInfo* colorAttachments = stackalloc RenderingAttachmentInfo[colorTargets.Length];

        for (int i = 0; i < colorTargets.Length; i++)
        {
            ref readonly ColorTargetInfo target = ref colorTargets[i];
            VkTexture texture = ((VkTexture) target.Texture);

            if (texture.IsSwapchainTexture)
            {
                _currentSwapchainTexture?.Transition(CommandBuffer, ImageLayout.ColorAttachmentOptimal,
                    ImageLayout.PresentSrcKhr);

                _currentSwapchainTexture = texture;
                _currentSwapchainTexture.Transition(CommandBuffer, ImageLayout.Undefined, ImageLayout.ColorAttachmentOptimal);
            }
            
            colorAttachments[i] = new RenderingAttachmentInfo()
            {
                SType = StructureType.RenderingAttachmentInfo,
                ImageView = texture.ImageView,
                ImageLayout = ImageLayout.ColorAttachmentOptimal,
                ClearValue = new ClearValue()
                {
                    Color = new ClearColorValue(target.ClearColor.R, target.ClearColor.G, target.ClearColor.B, target.ClearColor.A)
                },
                LoadOp = target.LoadOp.ToVk(),
                StoreOp = target.StoreOp.ToVk()
            };
        }

        Extent2D size = colorTargets[0].Texture.Size.ToVk();

        RenderingInfo renderingInfo = new()
        {
            SType = StructureType.RenderingInfo,
            ColorAttachmentCount = (uint) colorTargets.Length,
            PColorAttachments = colorAttachments,
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
            Y = 0,
            Width = size.Width,
            Height = size.Height,
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

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.FreeCommandBuffers(_device, _pool, 1, in CommandBuffer);
    }
}