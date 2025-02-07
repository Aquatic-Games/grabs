namespace grabs.Core;

public record struct Size2D
{
    public uint Width;

    public uint Height;

    public Size2D(uint width, uint height)
    {
        Width = width;
        Height = height;
    }
}