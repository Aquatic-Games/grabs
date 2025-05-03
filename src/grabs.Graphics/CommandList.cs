namespace grabs.Graphics;

public abstract class CommandList : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="CommandList"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Begin the command list. You <b>must</b> call this before issuing any commands.
    /// </summary>
    /// <remarks>This will reset the command list.</remarks>
    public abstract void Begin();

    /// <summary>
    /// End the command list. You <b>must</b> call this before executing it.
    /// </summary>
    public abstract void End();

    /// <summary>
    /// Begin a render pass with the given attachments.
    /// </summary>
    /// <param name="colorAttachments">The color attachments to use in this render pass. There must be at least 1 attachment.</param>
    public abstract void BeginRenderPass(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments);

    /// <summary>
    /// Begin a render pass with the given attachment.
    /// </summary>
    /// <param name="colorAttachment">The color attachment to use in the render pass.</param>
    public void BeginRenderPass(in ColorAttachmentInfo colorAttachment)
        => BeginRenderPass(new ReadOnlySpan<ColorAttachmentInfo>(in colorAttachment));
    
    /// <summary>
    /// End the currently active render pass.
    /// </summary>
    public abstract void EndRenderPass();

    /// <summary>
    /// Set the graphics <see cref="Pipeline"/> used on next draw.
    /// </summary>
    /// <param name="pipeline">The graphics <see cref="Pipeline"/> to use.</param>
    public abstract void SetGraphicsPipeline(Pipeline pipeline);

    /// <summary>
    /// Set the vertex <see cref="Buffer"/> used on next draw.
    /// </summary>
    /// <param name="slot">The slot to bind the buffer to.</param>
    /// <param name="buffer">The buffer to bind.</param>
    /// <param name="stride">The buffer's stride.</param>
    /// <param name="offset">The offset into the buffer.</param>
    public abstract void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset = 0);

    /// <summary>
    /// Set the index <see cref="Buffer"/> used on next draw.
    /// </summary>
    /// <param name="buffer">The buffer to bind.</param>
    /// <param name="format">The buffer's index format.</param>
    /// <param name="offset">The offset into the buffer.</param>
    public abstract void SetIndexBuffer(Buffer buffer, Format format, uint offset = 0);

    /// <summary>
    /// Draw with the given number of vertices.
    /// </summary>
    /// <param name="numVertices">The number of vertices to draw.</param>
    public abstract void Draw(uint numVertices);

    /// <summary>
    /// Draw with the given number of indices.
    /// </summary>
    /// <param name="numIndices">The number of indices to draw.</param>
    public abstract void DrawIndexed(uint numIndices);

    /// <summary>
    /// Dispose of this <see cref="CommandList"/>.
    /// </summary>
    public abstract void Dispose();
}