using grabs.Core;

namespace grabs.Graphics.Debugging;

internal sealed class DebugTexture : Texture
{
    public override bool IsDisposed
    {
        get => Texture.IsDisposed;
        protected set => throw new NotImplementedException();
    }
    
    public readonly Texture Texture;

    public readonly Format Format;
    
    public DebugTexture(Texture texture, Format format) : base(texture.Size)
    {
        Texture = texture;
        Format = format;
    }

    public override void Dispose()
    {
        Texture.Dispose();
    }
}