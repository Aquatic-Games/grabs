namespace grabs.Graphics;

/// <summary>
/// A swapchain, used for presentation, consists of at least 2 textures that can be rendered to in succession, that are
/// then presented to the attached window surface.
/// </summary>
public abstract class Swapchain : IDisposable
{
    public abstract Texture GetNextTexture();
    
    public abstract void Present();
    
    public abstract void Dispose();
}