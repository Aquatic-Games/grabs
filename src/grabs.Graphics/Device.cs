namespace grabs.Graphics;

/// <summary>
/// A logical device used for rendering.
/// </summary>
public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, ref readonly SwapchainDescription description);

    public abstract CommandList CreateCommandList();

    public abstract void ExecuteCommandList(CommandList cl);
    
    public abstract void Dispose();
}