namespace grabs.Graphics;

public abstract class Swapchain : IDisposable
{
    public abstract Texture GetNextTexture();
    
    public abstract void Present();
    
    public abstract void Dispose();
}