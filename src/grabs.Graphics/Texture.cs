using System;

namespace grabs.Graphics;

public abstract class Texture : IDisposable
{
    public readonly TextureDescription Description;

    protected Texture(TextureDescription description)
    {
        Description = description;
    }
    
    public abstract void Dispose();
}