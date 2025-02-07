using grabs.Core;

namespace grabs;

public abstract class Texture : IDisposable
{
    public abstract Size2D Size { get; }
    
    public abstract void Dispose();
}