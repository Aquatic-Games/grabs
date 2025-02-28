using grabs.Core;

namespace grabs.Graphics;

/// <summary>
/// Describes how a <see cref="Swapchain"/> should be created.
/// </summary>
public record struct SwapchainInfo
{
    /// <summary>
    /// The <see cref="grabs.Graphics.Surface"/> to use.
    /// </summary>
    public Surface Surface;
    
    /// <summary>
    /// The size, in pixels, of the swapchain.
    /// </summary>
    public Size2D Size;

    /// <summary>
    /// The <see cref="grabs.Graphics.Format"/> to use.
    /// </summary>
    public Format Format;

    /// <summary>
    /// The <see cref="grabs.Graphics.PresentMode"/> to use.
    /// </summary>
    public PresentMode PresentMode;

    /// <summary>
    /// The number of buffers that should be created. This number should generally be at least 2.
    /// </summary>
    public uint NumBuffers;
    
    /// <summary>
    /// Create a new <see cref="SwapchainInfo"/>.
    /// </summary>
    /// <param name="surface">The <see cref="grabs.Graphics.Surface"/> to use.</param>
    /// <param name="size">The size, in pixels, of the swapchain.</param>
    /// <param name="format">The <see cref="grabs.Graphics.Format"/> to use.</param>
    /// <param name="presentMode">The <see cref="grabs.Graphics.PresentMode"/> to use.</param>
    /// <param name="numBuffers">The number of buffers that should be created. This number should generally be at least
    /// 2.</param>
    public SwapchainInfo(Surface surface, Size2D size, Format format, PresentMode presentMode = PresentMode.Fifo,
        uint numBuffers = 2)
    {
        Surface = surface;
        Size = size;
        Format = format;
        PresentMode = presentMode;
        NumBuffers = numBuffers;
    }
}