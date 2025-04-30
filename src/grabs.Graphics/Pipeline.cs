namespace grabs.Graphics;

public abstract class Pipeline : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Pipeline"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this <see cref="Pipeline"/>.
    /// </summary>
    public abstract void Dispose();
}