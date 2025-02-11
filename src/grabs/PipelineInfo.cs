namespace grabs;

public record struct PipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public PipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
    }
}