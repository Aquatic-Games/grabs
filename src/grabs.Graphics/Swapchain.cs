using System;
using System.Drawing;

namespace grabs.Graphics;

public abstract class Swapchain : IDisposable
{
    public abstract PresentMode PresentMode { get; set; }

    public abstract Texture GetSwapchainTexture();

    public abstract void Resize(Size size);
    
    public abstract void Present();
    
    public abstract void Dispose();
}