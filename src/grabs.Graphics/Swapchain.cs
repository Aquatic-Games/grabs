namespace grabs.Graphics;

public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Swapchain"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Dispose of this <see cref="Swapchain"/>.
    /// </summary>
    public abstract void Dispose();
}