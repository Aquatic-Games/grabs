namespace grabs;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, in SwapchainDescription description);
    
    public abstract void Dispose();
}