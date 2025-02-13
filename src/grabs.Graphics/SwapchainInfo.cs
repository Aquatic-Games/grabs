using grabs.Core;

namespace grabs.Graphics;

public record struct SwapchainInfo
{
    public Size2D Size;

    public Format Format;

    public PresentMode PresentMode;

    public uint NumBuffers;

    public SwapchainInfo(Size2D size, Format format, PresentMode presentMode, uint numBuffers)
    {
        Size = size;
        Format = format;
        PresentMode = presentMode;
        NumBuffers = numBuffers;
    }
}