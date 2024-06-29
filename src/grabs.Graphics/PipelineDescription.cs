namespace grabs.Graphics;

public struct PipelineDescription
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public InputLayoutDescription[] InputLayout;

    public DepthStencilDescription DepthStencilState;

    public RasterizerDescription RasterizerState;

    public BlendDescription BlendState;

    public DescriptorLayout[] DescriptorLayouts;
    
    public PrimitiveType PrimitiveType;

    public PipelineDescription(ShaderModule vertexShader, ShaderModule pixelShader,
        InputLayoutDescription[] inputLayout, DepthStencilDescription depthStencilState,
        RasterizerDescription rasterizerState, BlendDescription blendState,
        DescriptorLayout[] descriptorLayouts, PrimitiveType primitiveType = PrimitiveType.TriangleList)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        InputLayout = inputLayout;
        DepthStencilState = depthStencilState;
        RasterizerState = rasterizerState;
        DescriptorLayouts = descriptorLayouts;
        BlendState = blendState;
        PrimitiveType = primitiveType;
    }
}