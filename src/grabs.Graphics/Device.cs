namespace grabs.Graphics;

public abstract class Device : IDisposable
{
    public abstract Adapter Adapter { get; }
    
    public abstract Swapchain CreateSwapchain(Surface surface, in SwapchainInfo info);

    public abstract CommandList CreateCommandList();

    public abstract ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint);

    public abstract unsafe Buffer CreateBuffer(in BufferInfo info, void* pData);

    public unsafe Buffer CreateBuffer(in BufferInfo info)
        => CreateBuffer(in info, null);

    public unsafe Buffer CreateBuffer<T>(BufferType type, in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(new BufferInfo(type, (uint) (data.Length * sizeof(T))), pData);
    }

    public Buffer CreateBuffer<T>(BufferType type, T[] data) where T : unmanaged
        => CreateBuffer<T>(type, data.AsSpan());
    
    public abstract Pipeline CreatePipeline(in PipelineInfo info);

    public abstract void ExecuteCommandList(CommandList list);

    public abstract void WaitForIdle();
    
    public abstract void Dispose();
}