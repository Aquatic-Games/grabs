namespace grabs.Graphics;

/// <summary>
/// A renderable surface that a <see cref="Swapchain"/> can be created for.
/// </summary>
public abstract class Surface : IDisposable
{
    /// <summary>
    /// Enumerate the supported surface formats for the given adapter.
    /// </summary>
    /// <param name="adapter">The adapter to use.</param>
    /// <returns>A list of supported surface formats.</returns>
    public abstract Format[] EnumerateSupportedFormats(in Adapter adapter);

    /// <summary>
    /// Calculate the optimal swapchain format for use with this surface.
    /// </summary>
    /// <param name="adapter">The adapter to use.</param>
    /// <param name="srgb">If true, SRGB formats will be checked for instead.</param>
    /// <returns>The calculated optimal format.</returns>
    /// <exception cref="NotSupportedException">Thrown if no supported formats are found.</exception>
    public Format GetOptimalSwapchainFormat(in Adapter adapter, bool srgb = false)
    {
        Format[] formats = EnumerateSupportedFormats(in adapter);

        Format format = Format.Unknown;
        foreach (Format fmt in formats)
        {
            bool isSrgb = fmt.IsSrgb();
            if ((isSrgb && srgb) || (!isSrgb && !srgb))
            {
                format = fmt;
                break;
            }
        }

        if (format == Format.Unknown)
            throw new NotSupportedException($"Could not find a swapchain format supporting srgb: {srgb}");

        return format;
    }
    
    /// <summary>
    /// Dispose of this surface.
    /// </summary>
    public abstract void Dispose();
}