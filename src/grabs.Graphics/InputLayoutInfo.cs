namespace grabs.Graphics;

public record struct InputLayoutInfo
{
    public Format Format;

    public uint Offset;

    public uint Slot;

    public InputLayoutInfo(Format format, uint offset, uint slot)
    {
        Format = format;
        Offset = offset;
        Slot = slot;
    }
}