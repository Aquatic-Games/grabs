namespace grabs.Graphics;

/// <summary>
/// Contains various types of <see cref="Buffer"/> that can be created.
/// </summary>
[Flags]
public enum BufferUsage
{
    /// <summary>
    /// A vertex buffer, used as the primary buffer for drawing 3D shapes.
    /// </summary>
    Vertex = 1 << 0,
    
    /// <summary>
    /// An index buffer, which can be used to reduce duplication in the vertex buffer.
    /// </summary>
    Index = 1 << 1,
    
    /// <summary>
    /// Constant buffer, used to store various "variables" to the shader. 
    /// </summary>
    Constant = 1 << 2,
    
    /// <summary>
    /// Transfer destination buffer.
    /// </summary>
    TransferDst = 1 << 8,
    
    /// <summary>
    /// Transfer source buffer.
    /// </summary>
    TransferSrc = 1 << 9,
    
    /// <summary>
    /// The buffer can be mapped for writing.
    /// </summary>
    MapWrite = 1 << 10,
    
    /// <summary>
    /// The buffer can be updated with <see cref="CommandList.UpdateBuffer"/>.
    /// </summary>
    UpdateBuffer = 1 << 16,
    
    /// <summary>
    /// The buffer is dynamic and will be updated one or more times per frame.
    /// </summary>
    Dynamic = UpdateBuffer | MapWrite
}