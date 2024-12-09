namespace grabs.Graphics;

public ref struct PipelineDescription
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<InputLayoutDescription> InputLayout;

    public PipelineDescription(ShaderModule vertexShader, ShaderModule pixelShader, ref readonly ReadOnlySpan<InputLayoutDescription> inputLayout)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        InputLayout = inputLayout;
    }
}