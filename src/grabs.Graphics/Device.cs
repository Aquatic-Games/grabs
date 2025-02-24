using System.Runtime.CompilerServices;

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

    public unsafe Buffer CreateBuffer<T>(BufferType type, in ReadOnlySpan<T> data, bool dynamic = false) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(new BufferInfo(type, (uint) (data.Length * sizeof(T)), dynamic), pData);
    }

    public Buffer CreateBuffer<T>(BufferType type, T[] data, bool dynamic = false) where T : unmanaged
        => CreateBuffer<T>(type, data.AsSpan(), dynamic);

    public unsafe Buffer CreateBuffer<T>(BufferType type, T data, bool dynamic = false) where T : unmanaged
        => CreateBuffer(new BufferInfo(type, (uint) sizeof(T), dynamic), Unsafe.AsPointer(ref data));
    
    public abstract Pipeline CreatePipeline(in PipelineInfo info);

    public abstract void ExecuteCommandList(CommandList list);

    public MappedData MapResource(MappableResource resource, MapType type)
        => resource.Map(type);

    public void UnmapResource(MappableResource resource)
        => resource.Unmap();

    public abstract void WaitForIdle();
    
    public abstract void Dispose();
}