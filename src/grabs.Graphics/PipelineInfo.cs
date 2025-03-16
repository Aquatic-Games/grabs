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
    /// The shader input layout.
    /// </summary>
    public ReadOnlySpan<InputElement> InputLayout;

    /// <summary>
    /// The descriptor layouts used.
    /// </summary>
    public ReadOnlySpan<DescriptorLayout> Descriptors;

    /// <summary>
    /// The constants that are defined in the shader.
    /// </summary>
    public ReadOnlySpan<ConstantLayout> Constants;

    /// <summary>
    /// Create a new <see cref="PipelineInfo"/>.
    /// </summary>
    /// <param name="vertexShader">The vertex shader to use.</param>
    /// <param name="pixelShader">The pixel shader to use.</param>
    /// <param name="colorAttachmentFormats">The formats of the color attachments that will be used with the pipeline.</param>
    /// <param name="inputLayout">The shader input layout.</param>
    /// <param name="descriptors">The descriptor layouts used.</param>
    /// <param name="constants">The constants that are defined in the shader.</param>
    public PipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader,
        ReadOnlySpan<Format> colorAttachmentFormats, ReadOnlySpan<InputElement> inputLayout,
        ReadOnlySpan<DescriptorLayout> descriptors, ReadOnlySpan<ConstantLayout> constants)
    {
        VertexShader = vertexShader;
        PixelShader = pixelShader;
        ColorAttachmentFormats = colorAttachmentFormats;
        InputLayout = inputLayout;
        Descriptors = descriptors;
        Constants = constants;
    }
}