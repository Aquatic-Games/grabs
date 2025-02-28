namespace grabs.Graphics;

/// <summary>
/// Describes how a vertex buffer will be used in a graphics pipeline.
/// </summary>
public struct VertexBufferInfo
{
    /// <summary>
    /// The binding slot of the vertex buffer.
    /// </summary>
    public uint Binding;

    /// <summary>
    /// The stride, in bytes, of the vertex buffer.
    /// </summary>
    public uint Stride;

    /// <summary>
    /// Create a new <see cref="VertexBufferInfo"/>
    /// </summary>
    /// <param name="binding">The binding slot of the vertex buffer.</param>
    /// <param name="stride">The stride, in bytes, of the vertex buffer.</param>
    public VertexBufferInfo(uint binding, uint stride)
    {
        Binding = binding;
        Stride = stride;
    }
}