namespace grabs.Graphics;

/// <summary>
/// Describes how a <see cref="Buffer"/> should be created.
/// </summary>
public record struct BufferInfo
{
    /// <summary>
    /// The type of buffer to create.
    /// </summary>
    public BufferType Type;

    /// <summary>
    /// The size of the buffer, in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// The way the buffer will be used.
    /// </summary>
    public BufferUsage Usage;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">The type of buffer to create.</param>
    /// <param name="size">The size of the buffer, in bytes.</param>
    /// <param name="usage">The way the buffer will be used.</param>
    public BufferInfo(BufferType type, uint size, BufferUsage usage)
    {
        Type = type;
        Size = size;
        Usage = usage;
    }
}