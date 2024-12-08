namespace grabs.Graphics;

/// <summary>
/// Describes how a <see cref="Swapchain"/> should be created.
/// </summary>
public record struct SwapchainDescription
{
    /// <summary>
    /// The size, in pixels, of the swapchain textures.
    /// </summary>
    public Size2D Size;

    /// <summary>
    /// The <see cref="grabs.Graphics.Format"/> that the swapchain textures should use.
    /// </summary>
    public Format Format;

    /// <summary>
    /// The number of buffers the swapchain should contain.
    /// </summary>
    /// <remarks>This value is only a hint. If an unsupported value is provided, a supported value will be automatically
    /// selected.</remarks>
    public uint NumBuffers;

    /// <summary>
    /// The <see cref="grabs.Graphics.PresentMode"/> to use for presentation.
    /// </summary>
    public PresentMode PresentMode;

    public SwapchainDescription(Size2D size, Format format, uint numBuffers, PresentMode presentMode)
    {
        Size = size;
        Format = format;
        NumBuffers = numBuffers;
        PresentMode = presentMode;
    }
}