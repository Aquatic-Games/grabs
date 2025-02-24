namespace grabs.Graphics;

/// <summary>
/// Contains various types of <see cref="Buffer"/> that can be created.
/// </summary>
public enum BufferType
{
    /// <summary>
    /// A vertex buffer, used as the primary buffer for drawing 3D shapes.
    /// </summary>
    Vertex,
    
    /// <summary>
    /// An index buffer, which can be used to reduce duplication in the vertex buffer.
    /// </summary>
    Index,
    
    /// <summary>
    /// Constant buffer, used to store various "variables" to the shader. 
    /// </summary>
    Constant
}