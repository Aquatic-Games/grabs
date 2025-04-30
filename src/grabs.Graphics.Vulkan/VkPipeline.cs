global using VulkanPipeline = Silk.NET.Vulkan.Pipeline;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkPipeline : Pipeline
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

        PipelineLayoutCreateInfo layoutInfo = new()
        {
            SType = StructureType.PipelineLayoutCreateInfo,
        };
        
        GrabsLog.Log("Creating pipeline layout");
        _vk.CreatePipelineLayout(_device, &layoutInfo, null, out Layout).Check("Create pipeline layout");

        VkShaderModule vertexModule = (VkShaderModule) info.VertexShader;
        using Utf8String pVertexEntryPoint = vertexModule.EntryPoint;
        
        VkShaderModule pixelModule = (VkShaderModule) info.PixelShader;
        using Utf8String pPixelEntryPoint = pixelModule.EntryPoint;

        PipelineShaderStageCreateInfo* shaders = stackalloc PipelineShaderStageCreateInfo[]
        {
            new PipelineShaderStageCreateInfo
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.VertexBit,
                Module = vertexModule.Module,
                PName = pVertexEntryPoint
            },
            new PipelineShaderStageCreateInfo
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.FragmentBit,
                Module = pixelModule.Module,
                PName = pPixelEntryPoint
            }
        };

        PipelineVertexInputStateCreateInfo vertexInput = new()
        {
            SType = StructureType.PipelineVertexInputStateCreateInfo,
        };
        
        // TODO: Primitive topology
        PipelineInputAssemblyStateCreateInfo inputAssembly = new()
        {
            SType = StructureType.PipelineInputAssemblyStateCreateInfo,
            Topology = PrimitiveTopology.TriangleList,
            PrimitiveRestartEnable = false
        };
        
        PipelineViewportStateCreateInfo viewportState = new()
        {
            SType = StructureType.PipelineViewportStateCreateInfo,
            ViewportCount = 1,
            ScissorCount = 1
        };

        PipelineRasterizationStateCreateInfo rasterizationState = new()
        {
            SType = StructureType.PipelineRasterizationStateCreateInfo,
            CullMode = CullModeFlags.None,
            LineWidth = 1.0f
        };

        PipelineMultisampleStateCreateInfo multisampleState = new()
        {
            SType = StructureType.PipelineMultisampleStateCreateInfo,
            RasterizationSamples = SampleCountFlags.Count1Bit
        };

        PipelineDepthStencilStateCreateInfo depthStencilState = new()
        {
            SType = StructureType.PipelineDepthStencilStateCreateInfo,
            DepthTestEnable = false,
            DepthWriteEnable = false
        };

        VulkanFormat* colorFormats = stackalloc VulkanFormat[info.ColorAttachments.Length];
        PipelineColorBlendAttachmentState* blendAttachments =
            stackalloc PipelineColorBlendAttachmentState[info.ColorAttachments.Length];
        
        for (int i = 0; i < info.ColorAttachments.Length; i++)
        {
            colorFormats[i] = info.ColorAttachments[i].Format.ToVk();
            blendAttachments[i] = new PipelineColorBlendAttachmentState()
            {
                BlendEnable = false
            };
        }

        PipelineRenderingCreateInfo renderingInfo = new()
        {
            SType = StructureType.PipelineRenderingCreateInfo,
            
            ColorAttachmentCount = (uint) info.ColorAttachments.Length,
            PColorAttachmentFormats = colorFormats
        };
        
        PipelineColorBlendStateCreateInfo blendState = new()
        {
            SType = StructureType.PipelineColorBlendStateCreateInfo,
            
            AttachmentCount = (uint) info.ColorAttachments.Length,
            PAttachments = blendAttachments
        };
        
        DynamicState* dynamicStates = stackalloc DynamicState[]
        {
            DynamicState.Viewport,
            DynamicState.Scissor
        };

        PipelineDynamicStateCreateInfo dynamicState = new()
        {
            SType = StructureType.PipelineDynamicStateCreateInfo,
            DynamicStateCount = 2,
            PDynamicStates = dynamicStates
        };

        GraphicsPipelineCreateInfo pipelineInfo = new()
        {
            SType = StructureType.GraphicsPipelineCreateInfo,
            PNext = &renderingInfo,

            StageCount = 2,
            PStages = shaders,

            PVertexInputState = &vertexInput,
            PInputAssemblyState = &inputAssembly,
            PViewportState = &viewportState,
            
            PRasterizationState = &rasterizationState,
            PMultisampleState = &multisampleState,
            PDepthStencilState = &depthStencilState,
            PColorBlendState = &blendState,
            
            PDynamicState = &dynamicState,
            
            Layout = Layout
        };
        
        GrabsLog.Log("Creating graphics pipeline.");
        _vk.CreateGraphicsPipelines(_device, new PipelineCache(), 1, &pipelineInfo, null, out Pipeline)
            .Check("Create graphics pipeline");
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        GrabsLog.Log("Destroying pipeline.");
        _vk.DestroyPipeline(_device, Pipeline, null);
        
        GrabsLog.Log("Destroying pipeline layout.");
        _vk.DestroyPipelineLayout(_device, Layout, null);
        
        ResourceTracker.DeregisterDeviceResource(_device, this);
    }
}