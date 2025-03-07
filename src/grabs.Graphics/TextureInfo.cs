using grabs.Core;

namespace grabs.Graphics;

public record struct TextureInfo
{
    public TextureType Type;

    public Size3D Size;

    public Format Format;

    public TextureUsage Usage;

    public TextureInfo(TextureType type, Size3D size, Format format, TextureUsage usage)
    {
        Type = type;
        Size = size;
        Format = format;
        Usage = usage;
    }

    public static TextureInfo Texture2D(Size2D size, Format format, TextureUsage usage)
        => new TextureInfo(TextureType.Texture2D, new Size3D(size.Width, size.Height, 1), format, usage);
}