namespace grabs.Graphics;

/// <summary>
/// Describes a render pass.
/// </summary>
public ref struct RenderPassInfo
{
    /// <summary>
    /// The color attachments to use for this render pass.
    /// </summary>
    public ReadOnlySpan<ColorAttachmentInfo> ColorAttachments;

    /// <summary>
    /// Create a new <see cref="RenderPassInfo"/>.
    /// </summary>
    /// <param name="colorAttachments">The color attachments to use for this render pass.</param>
    public RenderPassInfo(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        ColorAttachments = colorAttachments;
    }

    /// <summary>
    /// Create a new <see cref="RenderPassInfo"/>.
    /// </summary>
    /// <param name="colorAttachments">The color attachments to use for this render pass.</param>
    public RenderPassInfo(params ColorAttachmentInfo[] colorAttachments)
    {
        ColorAttachments = colorAttachments;
    }

    /// <summary>
    /// Create a new <see cref="RenderPassInfo"/> with a single color attachment.
    /// </summary>
    /// <param name="colorAttachment">The color attachment to use for this render pass.</param>
    public RenderPassInfo(in ColorAttachmentInfo colorAttachment)
    {
        ColorAttachments = new ReadOnlySpan<ColorAttachmentInfo>(in colorAttachment);
    }
}