using System;

namespace grabs.Graphics;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, in SwapchainDescription description);

    public abstract CommandList CreateCommandList();

    public abstract Pipeline CreatePipeline(in PipelineDescription description);

    public unsafe Buffer CreateBuffer(in BufferDescription description)
        => CreateBuffer(description, null);

    public unsafe Buffer CreateBuffer<T>(BufferType type, T data, bool dynamic = false) where T : unmanaged
        => CreateBuffer(new BufferDescription(type, (uint) sizeof(T), dynamic), data);
    
    public Buffer CreateBuffer<T>(in BufferDescription description, T data) where T : unmanaged
        => CreateBuffer(description, new ReadOnlySpan<T>(ref data));

    public unsafe Buffer CreateBuffer<T>(BufferType type, in ReadOnlySpan<T> data, bool dynamic = false) where T : unmanaged
        => CreateBuffer(new BufferDescription(type, (uint) (data.Length * sizeof(T)), dynamic), data);
    
    public unsafe Buffer CreateBuffer<T>(in BufferDescription description, in ReadOnlySpan<T> data) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(description, pData);
    }

    public abstract unsafe Buffer CreateBuffer(in BufferDescription description, void* pData);
    
    public abstract ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint);

    public abstract Framebuffer CreateFramebuffer(in ReadOnlySpan<Texture> colorTextures, Texture depthTexture = null);

    public abstract void ExecuteCommandList(CommandList list);
    
    public abstract void Dispose();
}