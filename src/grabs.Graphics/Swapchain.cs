namespace grabs.Graphics;

public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Swapchain"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Get the next swapchain texture that can be rendered to.
    /// </summary>
    /// <returns>The valid swapchain texture.</returns>
    public abstract Texture GetNextTexture();

    /// <summary>
    /// Present swapchain to the surface.
    /// </summary>
    public abstract void Present();
    
    /// <summary>
    /// Dispose of this <see cref="Swapchain"/>.
    /// </summary>
    public abstract void Dispose();
}