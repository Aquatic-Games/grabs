namespace grabs.Graphics;

/// <summary>
/// Describes how a graphics <see cref="Pipeline"/> should be created.
/// </summary>
/// <param name="vertexShader">The vertex shader to use.</param>
/// <param name="pixelShader">The pixel shader to use.</param>
/// <param name="colorAttachments">The color attachments that will be used with the pipeline.</param>
/// <param name="inputLayout">The vertex input layout.</param>
public ref struct GraphicsPipelineInfo(
    ShaderModule vertexShader,
    ShaderModule pixelShader,
    in ReadOnlySpan<ColorAttachmentDescription> colorAttachments,
    in ReadOnlySpan<InputElementDescription> inputLayout)
{
    /// <summary>
    /// The vertex shader to use.
    /// </summary>
    public ShaderModule VertexShader = vertexShader;

    /// <summary>
    /// The pixel shader to use.
    /// </summary>
    public ShaderModule PixelShader = pixelShader;

    /// <summary>
    /// The color attachments that will be used with the pipeline.
    /// </summary>
    public ReadOnlySpan<ColorAttachmentDescription> ColorAttachments = colorAttachments;

    /// <summary>
    /// The vertex input layout.
    /// </summary>
    public ReadOnlySpan<InputElementDescription> InputLayout = inputLayout;
}