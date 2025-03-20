using System.Diagnostics;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanCommandList : CommandList
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly CommandPool _pool;
    private readonly KhrPushDescriptor _pushDescriptor;

    private VulkanTexture? _currentSwapchainTexture;

    private readonly Sampler _tempSampler;
    
    public readonly CommandBuffer Buffer;

    public VulkanCommandList(Vk vk, VkDevice device, CommandPool pool, KhrPushDescriptor pushDescriptor)
    {
        _vk = vk;
        _device = device;
        _pool = pool;
        _pushDescriptor = pushDescriptor;
        
        CommandBufferAllocateInfo allocInfo = new CommandBufferAllocateInfo()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = pool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = 1
        };
        
        GrabsLog.Log("Allocating command buffer");
        _vk.AllocateCommandBuffers(device, &allocInfo, out Buffer).Check("Allocate command buffer");

        SamplerCreateInfo samplerInfo = new SamplerCreateInfo()
        {
            SType = StructureType.SamplerCreateInfo,
            MinFilter = Filter.Linear,
            MagFilter = Filter.Linear,
            AddressModeU = SamplerAddressMode.Repeat,
            AddressModeV = SamplerAddressMode.Repeat,
            AddressModeW = SamplerAddressMode.Repeat,
            MipmapMode = SamplerMipmapMode.Linear
        };

        _vk.CreateSampler(_device, &samplerInfo, null, out _tempSampler).Check("Create sampler");
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
        if (_currentSwapchainTexture != null)
        {
            _currentSwapchainTexture.TransitionImage(Buffer, ImageLayout.ColorAttachmentOptimal,
                ImageLayout.PresentSrcKhr);
            _currentSwapchainTexture = null;
        }
        
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

            if (texture.IsSwapchainTexture && _currentSwapchainTexture == null)
            {
                texture.TransitionImage(Buffer, ImageLayout.Undefined, ImageLayout.ColorAttachmentOptimal);
                _currentSwapchainTexture = texture;
            }

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

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0)
    {
        VkBuffer vkBuffer = ((VulkanBuffer) buffer).Buffer;

        ulong bufferOffset = offset;
        ulong bufferStride = stride;
        _vk.CmdBindVertexBuffers2(Buffer, slot, 1, &vkBuffer, &bufferOffset, null, &bufferStride);
    }

    public override void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0)
    {
        VulkanBuffer vkBuffer = (VulkanBuffer) buffer;

        IndexType type = format switch
        {
            Format.R16_UInt => IndexType.Uint16,
            Format.R32_UInt => IndexType.Uint32,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
        
        _vk.CmdBindIndexBuffer(Buffer, vkBuffer.Buffer, offset, type);
    }

    public override void PushConstant(Pipeline pipeline, ShaderStage stage, uint offset, uint size, void* pData)
    {
        VulkanPipeline vkPipeline = (VulkanPipeline) pipeline;
        _vk.CmdPushConstants(Buffer, vkPipeline.Layout, stage.ToVk(), offset, size, pData);
    }

    public override void UpdateBuffer(Buffer buffer, uint sizeInBytes, void* pData)
    {
        VulkanBuffer vkBuffer = (VulkanBuffer) buffer;
        vkBuffer.Update(Buffer, sizeInBytes, pData);
    }

    public override void PushDescriptors(uint slot, Pipeline pipeline, in ReadOnlySpan<Descriptor> descriptors)
    {
        VulkanPipeline vkPipeline = (VulkanPipeline) pipeline;
        
        int numDescriptors = descriptors.Length;
        WriteDescriptorSet* writeDescriptors = stackalloc WriteDescriptorSet[numDescriptors];

        DescriptorBufferInfo bufferInfo = default;
        
        for (int i = 0; i < numDescriptors; i++)
        {
            ref readonly Descriptor descriptor = ref descriptors[i];

            writeDescriptors[i] = new WriteDescriptorSet()
            {
                SType = StructureType.WriteDescriptorSet,
                DstBinding = descriptor.Slot,
                DescriptorCount = 1,
                DescriptorType = descriptor.Type.ToVk(),
            };

            switch (descriptor.Type)
            {
                case DescriptorType.ConstantBuffer:
                {
                    Debug.Assert(descriptor.Buffer != null);

                    VulkanBuffer vkBuffer = (VulkanBuffer) descriptor.Buffer;
                    
                    bufferInfo = new DescriptorBufferInfo()
                    {
                        Buffer = vkBuffer.Buffer,
                        Offset = vkBuffer.ReadOffset,
                        Range = vkBuffer.Info.Size
                    };

                    writeDescriptors[i].PBufferInfo = &bufferInfo;
                    break;
                }
                case DescriptorType.Texture:
                {
                    Debug.Assert(descriptor.Texture != null);
                    
                    VulkanTexture texture = (VulkanTexture) descriptor.Texture;

                    DescriptorImageInfo imageInfo = new DescriptorImageInfo()
                    {
                        ImageLayout = ImageLayout.ShaderReadOnlyOptimal,
                        ImageView = texture.ImageView,
                        Sampler = _tempSampler
                    };

                    writeDescriptors[i].PImageInfo = &imageInfo;
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        _pushDescriptor.CmdPushDescriptorSet(Buffer, PipelineBindPoint.Graphics, vkPipeline.Layout, slot,
            (uint) numDescriptors, writeDescriptors);
    }

    public override void Draw(uint numVertices)
    {
        _vk.CmdDraw(Buffer, numVertices, 1, 0, 0);
    }

    public override void DrawIndexed(uint numIndices)
    {
        _vk.CmdDrawIndexed(Buffer, numIndices, 1, 0, 0, 0);
    }

    public override void DrawIndexed(uint numIndices, uint startIndex, int baseVertex)
    {
        _vk.CmdDrawIndexed(Buffer, numIndices, 1, startIndex, baseVertex, 0);
    }

    public override void Dispose()
    {
        _vk.DestroySampler(_device, _tempSampler, null);
        
        fixed (CommandBuffer* buffer = &Buffer)
            _vk.FreeCommandBuffers(_device, _pool, 1, buffer);
    }
}