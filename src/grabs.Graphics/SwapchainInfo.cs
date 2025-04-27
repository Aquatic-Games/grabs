using grabs.Core;

namespace grabs.Graphics;

/// <summary>
/// Describes how a <see cref="Swapchain"/> should be created.
/// </summary>
/// <param name="surface">The <see cref="grabs.Graphics.Surface"/> that will be attached to the swapchain.</param>
/// <param name="size">The size, in pixels.</param>
/// <param name="format">The format of the color target.</param>
/// <param name="presentMode">The <see cref="grabs.Graphics.PresentMode"/> to use.</param>
/// <param name="numBuffers">The number of buffers (textures) available in the swapchain.</param>
public struct SwapchainInfo(Surface surface, Size2D size, Format format, PresentMode presentMode, uint numBuffers)
{
    /// <summary>
    /// The <see cref="grabs.Graphics.Surface"/> that will be attached to the swapchain.
    /// </summary>
    public Surface Surface = surface;

    /// <summary>
    /// The size, in pixels.
    /// </summary>
    public Size2D Size = size;

    /// <summary>
    /// The format of the color target.
    /// </summary>
    public Format Format = format;

    /// <summary>
    /// The <see cref="grabs.Graphics.PresentMode"/> to use.
    /// </summary>
    public PresentMode PresentMode = presentMode;

    /// <summary>
    /// The number of buffers (textures) available in the swapchain.
    /// </summary>
    public uint NumBuffers = numBuffers;
}