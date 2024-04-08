namespace grabs;

public abstract class Swapchain : IDisposable
{
    // TODO: HACK. This is temporary!!!
    public abstract ColorTarget GetColorTarget();
    
    public abstract void Present();
    
    public abstract void Dispose();
}