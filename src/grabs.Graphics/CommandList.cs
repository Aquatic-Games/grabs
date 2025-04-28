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
    /// Dispose of this <see cref="CommandList"/>.
    /// </summary>
    public abstract void Dispose();
}