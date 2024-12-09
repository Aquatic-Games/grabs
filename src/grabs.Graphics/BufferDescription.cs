namespace grabs.Graphics;

public record struct BufferDescription
{
    /// <summary>
    /// The buffer's type.
    /// </summary>
    public BufferType Type;

    /// <summary>
    /// The size of the buffer, in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// If true, the buffer will be placed in memory to allow for fast CPU writes. This also means the buffer can be
    /// mapped.
    /// </summary>
    public bool Dynamic;

    public BufferDescription(BufferType type, uint size, bool dynamic)
    {
        Type = type;
        Size = size;
        Dynamic = dynamic;
    }
}