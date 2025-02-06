namespace grabs;

public record struct SwapchainInfo
{
    public uint Width;

    public uint Height;

    public Format Format;

    public PresentMode PresentMode;

    public uint NumBuffers;

    public SwapchainInfo(uint width, uint height, Format format, PresentMode presentMode, uint numBuffers)
    {
        Width = width;
        Height = height;
        Format = format;
        PresentMode = presentMode;
        NumBuffers = numBuffers;
    }
}