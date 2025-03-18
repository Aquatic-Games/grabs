namespace grabs.Graphics;

/// <summary>
/// Represents various features supported by an <see cref="Adapter"/>.
/// </summary>
public readonly record struct AdapterFeatures
{
    /// <summary>
    /// Check whether anisotropic texture filtering is supported.
    /// </summary>
    public readonly bool TextureAnisotropy;

    /// <summary>
    /// Check if a geometry shader is supported.
    /// </summary>
    public readonly bool GeometryShader;

    /// <summary>
    /// Check if a compute shader is supported.
    /// </summary>
    public readonly bool ComputeShader;

    public AdapterFeatures(bool textureAnisotropy, bool geometryShader, bool computeShader)
    {
        TextureAnisotropy = textureAnisotropy;
        GeometryShader = geometryShader;
        ComputeShader = computeShader;
    }
}