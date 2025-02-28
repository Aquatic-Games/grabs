namespace grabs.Graphics;

/// <summary>
/// Describes how a graphics <see cref="Pipeline"/> should be created.
/// </summary>
public ref struct PipelineInfo
{
    /// <summary>
    /// The vertex shader to use.
    /// </summary>
    public ShaderModule VertexShader;

    /// <summary>
    /// The pixel shader to use.
    /// </summary>
    public ShaderModule PixelShader;

    /// <summary>
    /// The formats of the color attachments that will be used with the pipeline.
    /// </summary>
    public ReadOnlySpan<Format> ColorAttachmentFormats;

    /// <summary>
    /// The vertex buffer infos.
    /// </summary>
    public ReadOnlySpan<VertexBufferInfo> VertexBuffers;
    
    /// <summary>
    /// The shader input layout.
    /// </summary>
    public ReadOnlySpan<InputElement> InputLayout;

    /// <summary>
    /// Create a new <see cref="PipelineInfo"/>.
    /// </summary>
    /// <param name="vertexShader">The vertex shader to use.</param>
    /// <param name="pixelShader">The pixel shader to use.</param>
    /// <param name="colorAttachmentFormats">The formats of the color attachments that will be used with the pipeline.</param>
    /// <param name="vertexBuffers">The vertex buffer infos.</param>
    /// <param name="inputLayout">The shader input layout.</param>
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