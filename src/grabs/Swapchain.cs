namespace grabs;

public abstract class Swapchain : IDisposable
{
    public abstract void Present();
    
    public abstract void Dispose();
}