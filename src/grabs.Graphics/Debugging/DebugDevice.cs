namespace grabs.Graphics.Debugging;

internal sealed class DebugDevice(Device device) : Device
{
    public override bool IsDisposed
    {
        get => device.IsDisposed;
        protected set => throw new NotImplementedException();
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
        => new DebugSwapchain(device.CreateSwapchain(in info));
    
    public override CommandList CreateCommandList()
    {
        throw new NotImplementedException();
    }
    
    public override void ExecuteCommandList(CommandList cl)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}