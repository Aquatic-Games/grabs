namespace grabs.Core;

public record struct Size3D
{
    public uint Width;

    public uint Height;

    public uint Depth;

    public Size3D(uint width, uint height, uint depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
    }
}