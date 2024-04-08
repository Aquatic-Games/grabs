namespace grabs;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, in SwapchainDescription description);

    public abstract CommandList CreateCommandList();

    public Buffer CreateBuffer<T>(in BufferDescription description, T data) where T : unmanaged
        => CreateBuffer(description, new ReadOnlySpan<T>(ref data));
    
    public abstract Buffer CreateBuffer<T>(in BufferDescription description, in ReadOnlySpan<T> data) where T : unmanaged;

    public abstract void ExecuteCommandList(CommandList list);
    
    public abstract void Dispose();
}