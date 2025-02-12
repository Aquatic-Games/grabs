global using VkPipeline = Silk.NET.Vulkan.Pipeline;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanPipeline : Pipeline
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    public readonly PipelineLayout Layout;
    public readonly VkPipeline Pipeline;
    
    public VulkanPipeline(Vk vk, VkDevice device, ref readonly PipelineInfo info)
    {
        _vk = vk;
        _device = device;

        VulkanShaderModule vertexShader = (VulkanShaderModule) info.VertexShader;
        using PinnedString pVertexEntryPoint = vertexShader.EntryPoint;
        
        VulkanShaderModule pixelShader = (VulkanShaderModule) info.PixelShader;
        using PinnedString pPixelEntryPoint = pixelShader.EntryPoint;

        PipelineShaderStageCreateInfo* shaderStages = stackalloc PipelineShaderStageCreateInfo[]
        {
            new PipelineShaderStageCreateInfo()
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.VertexBit,
                Module = vertexShader.Module,
                PName = pVertexEntryPoint
            },
            new PipelineShaderStageCreateInfo()
            {
                SType = StructureType.PipelineShaderStageCreateInfo,
                Stage = ShaderStageFlags.FragmentBit,
                Module = pixelShader.Module,
                PName = pPixelEntryPoint
            }
        };

        PipelineVertexInputStateCreateInfo vertexInput = new PipelineVertexInputStateCreateInfo()
        {
            SType = StructureType.PipelineVertexInputStateCreateInfo,
        };

        PipelineInputAssemblyStateCreateInfo inputAssemblyState = new PipelineInputAssemblyStateCreateInfo()
        {
            SType = StructureType.PipelineInputAssemblyStateCreateInfo,
            // TODO: PrimitiveTopology
            Topology = PrimitiveTopology.TriangleList
        };

        PipelineViewportStateCreateInfo viewportState = new PipelineViewportStateCreateInfo()
        {
            SType = StructureType.PipelineViewportStateCreateInfo,
            ViewportCount = 1,
            ScissorCount = 1
        };

        PipelineRasterizationStateCreateInfo rasterizationState = new PipelineRasterizationStateCreateInfo()
        {
            SType = StructureType.PipelineRasterizationStateCreateInfo,
            DepthClampEnable = false,
            PolygonMode = PolygonMode.Fill,
            CullMode = CullModeFlags.None,
            LineWidth = 1
        };

        PipelineMultisampleStateCreateInfo multisampleState = new PipelineMultisampleStateCreateInfo()
        {
            SType = StructureType.PipelineMultisampleStateCreateInfo,
            SampleShadingEnable = false,
            RasterizationSamples = SampleCountFlags.Count1Bit
        };

        PipelineColorBlendAttachmentState blendAttachment = new PipelineColorBlendAttachmentState()
        {
            ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit |
                             ColorComponentFlags.ABit,
            BlendEnable = false
        };

        PipelineColorBlendStateCreateInfo blendState = new PipelineColorBlendStateCreateInfo()
        {
            SType = StructureType.PipelineColorBlendStateCreateInfo,
            AttachmentCount = 1,
            PAttachments = &blendAttachment
        };

        DynamicState* dynamicStates = stackalloc DynamicState[]
        {
            DynamicState.Viewport,
            DynamicState.Scissor
        };

        PipelineDynamicStateCreateInfo dynamicState = new PipelineDynamicStateCreateInfo()
        {
            SType = StructureType.PipelineDynamicStateCreateInfo,
            DynamicStateCount = 2,
            PDynamicStates = dynamicStates
        };

        VkFormat fmt = VkFormat.B8G8R8A8Unorm;

        PipelineRenderingCreateInfo renderingInfo = new PipelineRenderingCreateInfo()
        {
            SType = StructureType.PipelineRenderingCreateInfo,
            ColorAttachmentCount = 1,
            PColorAttachmentFormats = &fmt
        };

        PipelineLayoutCreateInfo layoutInfo = new PipelineLayoutCreateInfo()
        {
            SType = StructureType.PipelineLayoutCreateInfo,
        };

        _vk.CreatePipelineLayout(_device, &layoutInfo, null, out Layout)
            .Check("Create pipeline layout");

        GraphicsPipelineCreateInfo pipelineInfo = new GraphicsPipelineCreateInfo()
        {
            SType = StructureType.GraphicsPipelineCreateInfo,
            PNext = &renderingInfo,
            
            StageCount = 2,
            PStages = shaderStages,

            PVertexInputState = &vertexInput,
            PInputAssemblyState = &inputAssemblyState,
            PViewportState = &viewportState,
            PRasterizationState = &rasterizationState,
            PMultisampleState = &multisampleState,
            PColorBlendState = &blendState,
            PDynamicState = &dynamicState,
            
            Layout = Layout
        };

        _vk.CreateGraphicsPipelines(_device, new PipelineCache(), 1, &pipelineInfo, null, out Pipeline)
            .Check("Create pipeline");
    }
    
    public override void Dispose()
    {
        _vk.DestroyPipeline(_device, Pipeline, null);
        _vk.DestroyPipelineLayout(_device, Layout, null);
    }
}