namespace grabs.Graphics;

public struct BufferInfo(BufferUsage usage, uint size)
{
    public BufferUsage Usage = usage;

    public uint Size = size;
}