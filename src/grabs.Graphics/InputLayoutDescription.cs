namespace grabs.Graphics;

public record struct InputLayoutDescription
{
    public Format Format;

    public uint Offset;

    public uint Slot;

    public InputType Type;

    public InputLayoutDescription(Format format, uint offset, uint slot, InputType type)
    {
        Format = format;
        Offset = offset;
        Slot = slot;
        Type = type;
    }
}