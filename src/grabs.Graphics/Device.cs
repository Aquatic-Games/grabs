using System.Runtime.CompilerServices;

namespace grabs.Graphics;

/// <summary>
/// Represents a logical device that rendering commands can be issued to.
/// </summary>
public abstract class Device : IDisposable
{
    /// <summary>
    /// The <see cref="grabs.Graphics.Adapter"/> that was used to create this device.
    /// </summary>
    public abstract Adapter Adapter { get; }
    
    /// <summary>
    /// Create a swapchain for this device.
    /// </summary>
    /// <param name="info">Describe how the swapchain should be created.</param>
    /// <returns>The created <see cref="Swapchain"/>.</returns>
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    /// <summary>
    /// Create a command list for this device.
    /// </summary>
    /// <returns>The created <see cref="CommandList"/>.</returns>
    public abstract CommandList CreateCommandList();

    /// <summary>
    /// Create a shader module for use in pipeline creation.
    /// </summary>
    /// <param name="stage">The <see cref="ShaderStage"/> that this shader will be attached to.</param>
    /// <param name="spirv">The SPIR-V bytecode for this shader.</param>
    /// <param name="entryPoint">The shader's entry point.</param>
    /// <returns>The created <see cref="ShaderModule"/>.</returns>
    public abstract ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint);

    /// <summary>
    /// Create a buffer with the given description and data.
    /// </summary>
    /// <param name="info">The buffer's description.</param>
    /// <param name="pData">The data to initialize the buffer with. Can be null.</param>
    /// <returns>The created buffer.</returns>
    public abstract unsafe Buffer CreateBuffer(in BufferInfo info, void* pData);

    /// <summary>
    /// Create an empty buffer with the given description.
    /// </summary>
    /// <param name="info">The buffer's description.</param>
    /// <returns>The created buffer.</returns>
    public unsafe Buffer CreateBuffer(in BufferInfo info)
        => CreateBuffer(in info, null);

    /// <summary>
    /// Create a buffer with the given data.
    /// </summary>
    /// <param name="type">The <see cref="BufferType"/> to create the buffer with.</param>
    /// <param name="data">The data to initialize the buffer with.</param>
    /// <param name="usage">The way the buffer will be used.</param>
    /// <typeparam name="T">A primitive or sequential struct.</typeparam>
    /// <returns>The created buffer.</returns>
    public unsafe Buffer CreateBuffer<T>(BufferType type, in ReadOnlySpan<T> data, BufferUsage usage = BufferUsage.Default) where T : unmanaged
    {
        fixed (void* pData = data)
            return CreateBuffer(new BufferInfo(type, (uint) (data.Length * sizeof(T)), usage), pData);
    }

    /// <summary>
    /// Create a buffer with the given data.
    /// </summary>
    /// <param name="type">The <see cref="BufferType"/> to create the buffer with.</param>
    /// <param name="data">The data to initialize the buffer with.</param>
    /// <param name="usage">The way the buffer will be used.</param>
    /// <typeparam name="T">A primitive or sequential struct.</typeparam>
    /// <returns>The created buffer.</returns>
    public Buffer CreateBuffer<T>(BufferType type, T[] data, BufferUsage usage = BufferUsage.Default) where T : unmanaged
        => CreateBuffer<T>(type, data.AsSpan(), usage);

    /// <summary>
    /// Create a buffer with the given data.
    /// </summary>
    /// <param name="type">The <see cref="BufferType"/> to create the buffer with.</param>
    /// <param name="data">The data to initialize the buffer with.</param>
    /// <param name="usage">The way the buffer will be used.</param>
    /// <typeparam name="T">A primitive or sequential struct.</typeparam>
    /// <returns>The created buffer.</returns>
    public unsafe Buffer CreateBuffer<T>(BufferType type, T data, BufferUsage usage = BufferUsage.Default) where T : unmanaged
        => CreateBuffer(new BufferInfo(type, (uint) sizeof(T), usage), Unsafe.AsPointer(ref data));
    
    /// <summary>
    /// Create a graphics pipeline used for rendering.
    /// </summary>
    /// <param name="info">The pipeline's description.</param>
    /// <returns>The created pipeline.</returns>
    public abstract Pipeline CreatePipeline(in PipelineInfo info);

    /// <summary>
    /// Create a descriptor layout for use in pipelines and descriptor sets.
    /// </summary>
    /// <param name="info">The layout's description.</param>
    /// <returns>The created layout.</returns>
    public abstract DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutInfo info);

    /// <summary>
    /// Execute the commands in given command list.
    /// </summary>
    /// <param name="list">The <see cref="CommandList"/> to execute.</param>
    public abstract void ExecuteCommandList(CommandList list);

    /// <summary>
    /// Map the given resource into CPU accessible memory, so it can be written to/read from.
    /// </summary>
    /// <param name="resource">The <see cref="MappableResource"/> to map.</param>
    /// <param name="mode">The <see cref="MapMode"/> to map with, which defines how the buffer will be accessed.</param>
    /// <returns>A struct containing information about the mapped data.</returns>
    public MappedData MapResource(MappableResource resource, MapMode mode)
        => resource.Map(mode);

    /// <summary>
    /// Unmap a currently mapped resource.
    /// </summary>
    /// <param name="resource">The <see cref="MappableResource"/> to unmap.</param>
    public void UnmapResource(MappableResource resource)
        => resource.Unmap();

    /// <summary>
    /// Wait for the device to finish all queued actions.
    /// </summary>
    public abstract void WaitForIdle();
    
    /// <summary>
    /// Dispose of the device.
    /// </summary>
    public abstract void Dispose();
}