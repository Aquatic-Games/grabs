using grabs.Core;

namespace grabs.Graphics;

/// <summary>
/// A swapchain contains a series of <see cref="Texture"/>s (Images) that can be rendered to, then presented to a
/// <see cref="Surface"/>. 
/// </summary>
public abstract class Swapchain : IDisposable
{
    /// <summary>
    /// The surface <see cref="Format"/> used in this swapchain.
    /// </summary>
    public abstract Format SwapchainFormat { get; }
    
    public abstract Size2D Size { get; }

    /// <summary>
    /// Get the next texture used for rendering. Depending on the <see cref="SwapchainInfo.PresentMode"/>, this may
    /// block the thread.
    /// </summary>
    /// <returns></returns>
    public abstract Texture GetNextTexture();
    
    /// <summary>
    /// Present the current Texture to the surface.
    /// </summary>
    public abstract void Present();
    
    /// <summary>
    /// Dispose of this swapchain.
    /// </summary>
    public abstract void Dispose();
}