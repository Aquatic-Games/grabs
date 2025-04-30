namespace grabs.Graphics;

public abstract class ShaderModule : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="ShaderModule"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this <see cref="ShaderModule"/>.
    /// </summary>
    public abstract void Dispose();
}