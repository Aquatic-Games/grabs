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
    /// If <see langword="true" />, the buffer will be created in a region of memory accessible by the CPU, to allow
    /// for fast updates. Enable if you are updating the buffer every frame.
    /// </summary>
    public bool Dynamic;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">The type of buffer to create.</param>
    /// <param name="size">The size of the buffer, in bytes.</param>
    /// <param name="dynamic">If <see langword="true" />, the buffer will be created in a region of memory accessible by
    /// the CPU, to allow for fast updates. Enable if you are updating the buffer every frame.</param>
    public BufferInfo(BufferType type, uint size, bool dynamic = false)
    {
        Type = type;
        Size = size;
        Dynamic = dynamic;
    }
}