namespace grabs;

public ref struct PipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<Format> ColorAttachmentFormats;

    public PipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader, in ReadOnlySpan<Format> colorAttachmentFormats)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorAttachmentFormats = colorAttachmentFormats;
    }

    public PipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader, in Format colorAttachmentFormat)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorAttachmentFormats = new ReadOnlySpan<Format>(in colorAttachmentFormat);
    }
}