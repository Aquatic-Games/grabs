namespace grabs.Graphics;

/// <summary>
/// Describes how a <see cref="Swapchain"/> should be created.
/// </summary>
public record struct SwapchainDescription
{
    /// <summary>
    /// The width, in pixels, of the swapchain textures.
    /// </summary>
    public uint Width;

    /// <summary>
    /// The height, in pixels, of the swapchain textures.
    /// </summary>
    public uint Height;

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

    public SwapchainDescription(uint width, uint height, Format format, uint numBuffers, PresentMode presentMode)
    {
        Width = width;
        Height = height;
        Format = format;
        NumBuffers = numBuffers;
        PresentMode = presentMode;
    }
}