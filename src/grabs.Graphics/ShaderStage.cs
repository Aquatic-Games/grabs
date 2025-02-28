namespace grabs.Graphics;

/// <summary>
/// Defines various stages a shader can be.
/// </summary>
[Flags]
public enum ShaderStage
{
    /// <summary>
    /// No shader stage. Do not use.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// A vertex shader.
    /// </summary>
    Vertex,
    
    /// <summary>
    /// A pixel (fragment) shader.
    /// </summary>
    Pixel,
    
    /// <summary>
    /// Vertex and pixel shader stages.
    /// </summary>
    VertexPixel = Vertex | Pixel
}