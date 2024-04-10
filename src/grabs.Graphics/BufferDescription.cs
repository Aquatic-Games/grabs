namespace grabs.Graphics;

public struct BufferDescription
{
    public BufferType Type;

    public uint SizeInBytes;

    // TODO: Replace this with BufferUsage instead, allowing for staging buffers.
    public bool Dynamic;

    public BufferDescription(BufferType type, uint sizeInBytes, bool dynamic = false)
    {
        Type = type;
        SizeInBytes = sizeInBytes;
        Dynamic = dynamic;
    }
}