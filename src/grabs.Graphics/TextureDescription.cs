namespace grabs.Graphics;

public struct TextureDescription
{
    public TextureType Type;
    public uint Width;
    public uint Height;
    public uint MipLevels;
    public Format Format;
    public TextureUsage Usage;
    
    public TextureDescription(TextureType type, uint width, uint height, uint mipLevels, Format format, TextureUsage usage)
    {
        Type = type;
        Width = width;
        Height = height;
        MipLevels = mipLevels;
        Format = format;
        Usage = usage;
    }

    public static TextureDescription Texture2D(uint width, uint height, uint mipLevels, Format format, TextureUsage usage)
        => new TextureDescription(TextureType.Texture2D, width, height, mipLevels, format, usage);

    public static TextureDescription Cubemap(uint width, uint height, uint mipLevels, Format format, TextureUsage usage)
        => new TextureDescription(TextureType.Cubemap, width, height, mipLevels, format, usage);
}