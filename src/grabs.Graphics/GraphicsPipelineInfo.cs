namespace grabs.Graphics;

/// <summary>
/// Describes how a graphics <see cref="Pipeline"/> should be created.
/// </summary>
/// <param name="vertexShader">The vertex shader to use.</param>
/// <param name="pixelShader">The pixel shader to use.</param>
public struct GraphicsPipelineInfo(ShaderModule vertexShader, ShaderModule pixelShader)
{
    /// <summary>
    /// The vertex shader to use.
    /// </summary>
    public ShaderModule VertexShader = vertexShader;

    /// <summary>
    /// The pixel shader to use.
    /// </summary>
    public ShaderModule PixelShader = pixelShader;
}