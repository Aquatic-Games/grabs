namespace grabs.Graphics;

/// <summary>
/// Describes how a buffer can be used.
/// </summary>
[Flags]
public enum BufferUsage
{
    /// <summary>
    /// The buffer will not be used.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// The buffer will be a vertex buffer.
    /// </summary>
    Vertex = 1 << 0,
    
    /// <summary>
    /// The buffer will be an index buffer.
    /// </summary>
    Index = 1 << 1,
    
    /// <summary>
    /// The buffer will be a constant buffer.
    /// </summary>
    Constant = 1 << 2,
    
    /// <summary>
    /// The buffer will be the source for copy operations.
    /// </summary>
    CopySrc = 1 << 8,
    
    /// <summary>
    /// The buffer will be the destination of copy operations.
    /// </summary>
    CopyDst = 1 << 9
}