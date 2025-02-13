using grabs.VulkanMemoryAllocator;

namespace grabs.Graphics;

public record struct BufferInfo
{
    public BufferType Type;

    public uint Size;

    public BufferInfo(BufferType type, uint size)
    {
        Type = type;
        Size = size;
    }
}