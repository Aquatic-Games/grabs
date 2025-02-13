namespace grabs.Graphics;

public ref struct PipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<Format> ColorAttachmentFormats;

    public ReadOnlySpan<InputLayoutInfo> InputLayout;

    // TODO: This is temporary
    public uint Stride;

    public PipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader,
        in ReadOnlySpan<Format> colorAttachmentFormats, in ReadOnlySpan<InputLayoutInfo> inputLayout)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorAttachmentFormats = colorAttachmentFormats;
        InputLayout = inputLayout;
    }

    public PipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader, in Format colorAttachmentFormat,
        in ReadOnlySpan<InputLayoutInfo> inputLayout)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorAttachmentFormats = new ReadOnlySpan<Format>(in colorAttachmentFormat);
        InputLayout = inputLayout;
    }
}