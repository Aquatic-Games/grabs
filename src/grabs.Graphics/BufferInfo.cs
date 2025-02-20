using grabs.VulkanMemoryAllocator;

namespace grabs.Graphics;

public record struct BufferInfo
{
    public BufferType Type;

    public uint Size;

    public bool Dynamic;

    public BufferInfo(BufferType type, uint size, bool dynamic = false)
    {
        Type = type;
        Size = size;
        Dynamic = dynamic;
    }
}