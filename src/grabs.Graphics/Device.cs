using System.Runtime.CompilerServices;

namespace grabs.Graphics;

/// <summary>
/// Represents a logical device that can have commands issued to it.
/// </summary>
public abstract class Device : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Device"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Gets the <see cref="grabs.Graphics.ShaderFormat"/> that this device accepts.
    /// </summary>
    public abstract ShaderFormat ShaderFormat { get; }

    /// <summary>
    /// Create a <see cref="Swapchain"/>.
    /// </summary>
    /// <param name="info">The <see cref="SwapchainInfo"/> used to describe the swapchain.</param>
    /// <returns>The created <see cref="Swapchain"/>.</returns>
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    /// <summary>
    /// Create a <see cref="CommandList"/>.
    /// </summary>
    /// <returns>The created <see cref="CommandList"/>.</returns>
    public abstract CommandList CreateCommandList();

    /// <summary>
    /// Create a <see cref="ShaderModule"/>.
    /// </summary>
    /// <param name="stage">The shader stage of the module.</param>
    /// <param name="code">The shader code.</param>
    /// <param name="entryPoint">The entry point.</param>
    /// <returns>The created <see cref="ShaderModule"/>.</returns>
    /// <remarks>This must be in the same format as the <see cref="ShaderFormat"/>.</remarks>
    public abstract ShaderModule CreateShaderModule(ShaderStage stage, byte[] code, string entryPoint);

    /// <summary>
    /// Create a graphics <see cref="Pipeline"/>.
    /// </summary>
    /// <param name="info">The <see cref="GraphicsPipelineInfo"/> used to describe the pipeline.</param>
    /// <returns>The created <see cref="Pipeline"/>.</returns>
    public abstract Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info);

    public abstract unsafe Buffer CreateBuffer(in BufferInfo info, void* pData);

    public unsafe Buffer CreateBuffer(in BufferInfo info)
        => CreateBuffer(in info, null);

    public unsafe Buffer CreateBuffer<T>(BufferUsage usage, T data) where T : unmanaged
    {
        BufferInfo info = new(usage, (uint) sizeof(T));
        return CreateBuffer(in info, Unsafe.AsPointer(ref data));
    }

    public unsafe Buffer CreateBuffer<T>(BufferUsage usage, in ReadOnlySpan<T> data) where T : unmanaged
    {
        BufferInfo info = new(usage, (uint) (data.Length * sizeof(T)));

        fixed (void* pData = data)
            return CreateBuffer(in info, pData);
    }

    public Buffer CreateBuffer<T>(BufferUsage usage, T[] data) where T : unmanaged
        => CreateBuffer<T>(usage, data.AsSpan());

    /// <summary>
    /// Execute the commands in the command list.
    /// </summary>
    /// <param name="cl">The <see cref="CommandList"/> to execute.</param>
    public abstract void ExecuteCommandList(CommandList cl);

    /// <summary>
    /// Dispose of this <see cref="Device"/>.
    /// </summary>
    public abstract void Dispose();
}