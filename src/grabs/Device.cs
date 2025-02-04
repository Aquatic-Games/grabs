namespace grabs;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, in SwapchainInfo info);
    
    public abstract void Dispose();
}