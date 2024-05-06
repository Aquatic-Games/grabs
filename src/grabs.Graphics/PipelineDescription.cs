namespace grabs.Graphics;

public struct PipelineDescription
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public InputLayoutDescription[] InputLayout;

    public DepthStencilDescription DepthStencilDescription;

    public PipelineDescription(ShaderModule vertexShader, ShaderModule pixelShader, InputLayoutDescription[] inputLayout, DepthStencilDescription depthStencilDescription)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        InputLayout = inputLayout;
        DepthStencilDescription = depthStencilDescription;
    }
}