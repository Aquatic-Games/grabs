using grabs.Core;

namespace grabs.Graphics;

public abstract class Texture : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Texture"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// The size in pixels.
    /// </summary>
    public readonly Size2D Size;

    protected Texture(Size2D size)
    {
        Size = size;
    }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public abstract void Dispose();
}