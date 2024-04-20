using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Texture : Texture
{
    private readonly GL _gl;

    public readonly uint Texture;
    public readonly TextureTarget Target;

    public unsafe GL43Texture(GL gl, in TextureDescription description, void* pData)
    {
        _gl = gl;

        Target = description.Type switch
        {
            TextureType.Texture2D => TextureTarget.Texture2D,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Texture = _gl.GenTexture();
        _gl.BindTexture(Target, Texture);

        // If mip levels are 0 then calculate the number of mip levels.
        uint mipLevels = description.MipLevels == 0
            ? (uint) float.Floor(float.Log2(uint.Max(description.Width, description.Height))) + 1
            : description.MipLevels;

        // TODO: Implement all the formats. I would do it now but brain no worky
        (SizedInternalFormat iFmt, PixelFormat fmt, PixelType pType) = description.Format switch
        {
            Format.R8G8B8A8_UNorm => (SizedInternalFormat.Rgba8, PixelFormat.Rgba, PixelType.UnsignedByte),
            _ => throw new NotImplementedException()
        };

        switch (description.Type)
        {
            case TextureType.Texture2D:
                _gl.TexStorage2D(Target, mipLevels, iFmt, description.Width, description.Height);
                _gl.TexSubImage2D(Target, 0, 0, 0, description.Width, description.Height, fmt, pType, pData);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Dispose()
    {
        _gl.DeleteTexture(Texture);
    }
}