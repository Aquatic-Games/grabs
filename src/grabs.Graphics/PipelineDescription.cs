namespace grabs.Graphics;

public struct PipelineDescription
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public InputLayoutDescription[] InputLayout;

    public PipelineDescription(ShaderModule vertexShader, ShaderModule pixelShader, InputLayoutDescription[] inputLayout)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        InputLayout = inputLayout;
    }
}