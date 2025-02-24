namespace grabs.Graphics;

public ref struct PipelineInfo
{
    public ShaderModule VertexShader;

    public ShaderModule PixelShader;

    public ReadOnlySpan<Format> ColorAttachmentFormats;

    public ReadOnlySpan<VertexBufferInfo> VertexBuffers;
    
    public ReadOnlySpan<InputElement> InputLayout;

    public PipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader,
        ReadOnlySpan<Format> colorAttachmentFormats, ReadOnlySpan<VertexBufferInfo> vertexBuffers,
        ReadOnlySpan<InputElement> inputLayout)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorAttachmentFormats = colorAttachmentFormats;
        VertexBuffers = vertexBuffers;
        InputLayout = inputLayout;
    }
}