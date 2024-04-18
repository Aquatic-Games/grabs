using System;

namespace grabs.Graphics;

public abstract class Swapchain : IDisposable
{
    public abstract PresentMode PresentMode { get; set; }

    public abstract Texture GetSwapchainTexture();
    
    public abstract void Present();
    
    public abstract void Dispose();
}