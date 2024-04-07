namespace grabs;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(in SwapchainDescription description);
    
    public abstract void Dispose();
}