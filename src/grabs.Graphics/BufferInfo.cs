namespace grabs.Graphics;

/// <summary>
/// Describes how a <see cref="Buffer"/> should be created.
/// </summary>
public record struct BufferInfo
{
    /// <summary>
    /// The type of buffer to create.
    /// </summary>
    public BufferUsage Usage;

    /// <summary>
    /// The size of the buffer, in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="usage">The type of buffer to create.</param>
    /// <param name="size">The size of the buffer, in bytes.</param>
    public BufferInfo(BufferUsage usage, uint size)
    {
        Usage = usage;
        Size = size;
    }
}