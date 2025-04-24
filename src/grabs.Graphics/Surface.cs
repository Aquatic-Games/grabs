namespace grabs.Graphics;

public abstract class Surface : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Surface"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Dispose of this <see cref="Surface"/>.
    /// </summary>
    public abstract void Dispose();
}