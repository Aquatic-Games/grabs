using System.Runtime.CompilerServices;

namespace grabs.Graphics;

/// <summary>
/// A command list contains a list of instructions used for rendering, which can then be executed by the GPU. 
/// </summary>
public abstract class CommandList : IDisposable
{
    /// <summary>
    /// Begin the command list, so it can accept commands.
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// End and finalize the command list.
    /// </summary>
    public abstract void End();

    /// <summary>
    /// Begin a render pass with the given <see cref="RenderPassInfo"/>.
    /// </summary>
    /// <param name="info">The <see cref="RenderPassInfo"/> to describe the render pass.</param>
    public abstract void BeginRenderPass(in RenderPassInfo info);

    /// <summary>
    /// End the current render pass.
    /// </summary>
    public abstract void EndRenderPass();

    /// <summary>
    /// Set the viewport for rendering.
    /// </summary>
    /// <param name="viewport">The <see cref="Viewport"/> that will be used for rendering.</param>
    public abstract void SetViewport(in Viewport viewport);

    /// <summary>
    /// Set the <see cref="Pipeline"/> to use on next draw.
    /// </summary>
    /// <param name="pipeline">The <see cref="Pipeline"/> to use.</param>
    public abstract void SetPipeline(Pipeline pipeline);

    /// <summary>
    /// Set the vertex buffer to use on next draw.
    /// </summary>
    /// <param name="slot">The slot to bind the buffer at.</param>
    /// <param name="buffer">The buffer to use.</param>
    /// <param name="stride">The stride, in bytes, of the buffer.</param>
    /// <param name="offset">The offset, in bytes, to bind the buffer at.</param>
    public abstract void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0);

    /// <summary>
    /// Set the index buffer to use on next draw.
    /// </summary>
    /// <param name="buffer">The index buffer to use.</param>
    /// <param name="format">The elements type. Valid values are <see cref="Format.R32_UInt"/> and <see cref="Format.R16_UInt"/>.</param>
    /// <param name="offset">The offset, in bytes, to bind the buffer at.</param>
    public abstract void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0);

    public abstract unsafe void UpdateBuffer(Buffer buffer, uint sizeInBytes, void* pData);

    public unsafe void UpdateBuffer<T>(Buffer buffer, T data) where T : unmanaged
        => UpdateBuffer(buffer, (uint) sizeof(T), Unsafe.AsPointer(ref data));

    public unsafe void UpdateBuffer<T>(Buffer buffer, in ReadOnlySpan<T> data) where T : unmanaged
    {
        uint dataSize = (uint) (data.Length * sizeof(T));
        
        fixed (void* pData = data)
            UpdateBuffer(buffer, dataSize, pData);
    }

    public void UpdateBuffer<T>(Buffer buffer, T[] data) where T : unmanaged
        => UpdateBuffer<T>(buffer, data.AsSpan());

    public abstract void PushDescriptors(uint slot, Pipeline pipeline, in ReadOnlySpan<Descriptor> descriptors);

    public void PushDescriptor(uint slot, Pipeline pipeline, in Descriptor descriptor)
        => PushDescriptors(slot, pipeline, new ReadOnlySpan<Descriptor>(in descriptor));

    /// <summary>
    /// Draw vertices with the given number of vertices.
    /// </summary>
    /// <param name="numVertices">The number of vertices to draw.</param>
    public abstract void Draw(uint numVertices);

    /// <summary>
    /// Draw indexed vertices from the given number of indices.
    /// </summary>
    /// <param name="numIndices">The number of indices to draw.</param>
    public abstract void DrawIndexed(uint numIndices);
    
    /// <summary>
    /// Dispose of the command list.
    /// </summary>
    public abstract void Dispose();
}