using grabs.Core;

namespace grabs.Graphics;

/// <summary>
/// Describes how a <see cref="Swapchain"/> should be created.
/// </summary>
/// <param name="Surface">The <see cref="grabs.Graphics.Surface"/> that will be attached to the swapchain.</param>
/// <param name="Size">The size, in pixels.</param>
/// <param name="Format">The format of the color target.</param>
/// <param name="PresentMode">The <see cref="grabs.Graphics.PresentMode"/> to use.</param>
/// <param name="NumBuffers">The number of buffers (textures) available in the swapchain.</param>
public record struct SwapchainInfo(Surface Surface, Size2D Size, Format Format, PresentMode PresentMode, uint NumBuffers);