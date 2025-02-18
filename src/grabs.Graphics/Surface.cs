namespace grabs.Graphics;

public abstract class Surface : IDisposable
{
    public abstract Format[] EnumerateSupportedFormats(in Adapter adapter);

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
    
    public abstract void Dispose();
}