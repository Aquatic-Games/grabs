using grabs.Core;

namespace grabs.Graphics;

public abstract class Texture : IDisposable
{
    public abstract Size2D Size { get; }
    
    public abstract void Dispose();
}