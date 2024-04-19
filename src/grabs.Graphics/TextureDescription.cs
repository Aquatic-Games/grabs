namespace grabs.Graphics;

public struct TextureDescription
{
    public TextureType Type;
    public uint Width;
    public uint Height;
    public uint MipLevels;
    public Format Format;

    public TextureDescription(TextureType type, uint width, uint height, uint mipLevels, Format format)
    {
        Type = type;
        Width = width;
        Height = height;
        MipLevels = mipLevels;
        Format = format;
    }

    public static TextureDescription Texture2D(uint width, uint height, uint mipLevels, Format format)
        => new TextureDescription(TextureType.Texture2D, width, height, mipLevels, format);
}