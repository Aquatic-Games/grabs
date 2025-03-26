using grabs.Core;

namespace grabs.Graphics.OpenGL;

internal sealed class GLSwapchain : Swapchain
{
    public override Format SwapchainFormat { get; }
    
    public override Size2D Size { get; }
    
    public override Texture GetNextTexture()
    {
        throw new NotImplementedException();
    }
    
    public override void Present()
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}