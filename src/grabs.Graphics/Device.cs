using System.Runtime.CompilerServices;

namespace grabs.Graphics;

/// <summary>
/// A logical device used for rendering.
/// </summary>
public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, ref readonly SwapchainDescription description);

    public abstract CommandList CreateCommandList();

    /// <summary>
    /// Create a GPU buffer with the given data.
    /// </summary>
    /// <param name="description">The buffer's description.</param>
    /// <param name="data">The data the buffer should contain. Must be <b>at least</b> as large as
    /// <see cref="BufferDescription.Size"/>. Can be null.</param>
    /// <returns>The created buffer.</returns>
    public abstract unsafe Buffer CreateBuffer(ref readonly BufferDescription description, void* data);

    /// <summary>
    /// Create an empty GPU buffer from the description.
    /// </summary>
    /// <param name="description">The buffer's description.</param>
    /// <returns>The created buffer.</returns>
    public unsafe Buffer CreateBuffer(ref readonly BufferDescription description)
        => CreateBuffer(in description, null);

    public unsafe Buffer CreateBuffer<T>(BufferType type, T data, bool dynamic = false) where T : unmanaged
    {
        BufferDescription description = new BufferDescription(type, (uint) sizeof(T), dynamic);
        return CreateBuffer(ref description, Unsafe.AsPointer(ref data));
    }

    public unsafe Buffer CreateBuffer<T>(BufferType type, ref readonly ReadOnlySpan<T> data, bool dynamic = false)
        where T : unmanaged
    {
        BufferDescription description = new BufferDescription(type, (uint) (sizeof(T) * data.Length), dynamic);

        fixed (void* pData = data)
            return CreateBuffer(ref description, pData);
    }

    public abstract ShaderModule CreateShaderModule(ShaderStage stage, ref readonly ReadOnlySpan<byte> spirv,
        string entryPoint);

    public ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        ReadOnlySpan<byte> spanSpv = spirv.AsSpan();
        return CreateShaderModule(stage, in spanSpv, entryPoint);
    }
    
    public unsafe Buffer CreateBuffer<T>(BufferType type, T[] data, bool dynamic = false) where T : unmanaged
    {
        BufferDescription description = new BufferDescription(type, (uint) (sizeof(T) * data.Length), dynamic);

        fixed (void* pData = data)
            return CreateBuffer(ref description, pData);
    }

    public abstract void ExecuteCommandList(CommandList cl);
    
    public abstract void Dispose();
}