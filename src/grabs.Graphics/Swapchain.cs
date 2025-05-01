namespace grabs.Graphics;

public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Swapchain"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Gets the buffer <see cref="Format"/> of the <see cref="Texture"/> returned by <see cref="GetNextTexture"/>.
    /// </summary>
    public abstract Format BufferFormat { get; }

    /// <summary>
    /// Get the next swapchain texture that can be rendered to. You must call this before rendering to the swapchain.
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