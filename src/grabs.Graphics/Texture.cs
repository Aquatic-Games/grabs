namespace grabs.Graphics;

public abstract class Texture : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Texture"/> is disposed.
    /// </summary>
    public bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this <see cref="Texture"/>.
    /// </summary>
    public abstract void Dispose();
}