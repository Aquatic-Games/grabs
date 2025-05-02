namespace grabs.Graphics;

public abstract class Buffer : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Buffer"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this <see cref="Buffer"/>.
    /// </summary>
    public abstract void Dispose();
}