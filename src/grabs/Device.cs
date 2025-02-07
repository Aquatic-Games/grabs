namespace grabs;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, in SwapchainInfo info);

    public abstract CommandList CreateCommandList();

    public abstract void ExecuteCommandList(CommandList list);

    public abstract void WaitForIdle();
    
    public abstract void Dispose();
}