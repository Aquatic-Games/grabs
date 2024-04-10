using System;

namespace grabs;

public abstract class Swapchain : IDisposable
{
    public abstract PresentMode PresentMode { get; set; }
    
    // TODO: HACK. This is temporary!!!
    public abstract ColorTarget GetColorTarget();
    
    public abstract void Present();
    
    public abstract void Dispose();
}