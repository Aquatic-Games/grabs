namespace grabs.Graphics;

public abstract class Swapchain : IDisposable
{
    public abstract Format SwapchainFormat { get; }
    
    public abstract Texture GetNextTexture();
    
    public abstract void Present();
    
    public abstract void Dispose();
}