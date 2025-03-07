namespace grabs.Graphics;

public record struct Descriptor
{
    public uint Slot;
    
    public DescriptorType Type;

    public Buffer? Buffer;

    public Descriptor(uint slot, DescriptorType type, Buffer? buffer = null)
    {
        Slot = slot;
        Type = type;
        Buffer = buffer;
    }
}