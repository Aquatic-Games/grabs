namespace grabs;

public record struct SwapchainInfo
{
    public uint Width;

    public uint Height;

    public Format Format;

    public uint NumBuffers;

    public SwapchainInfo(uint width, uint height, Format format, uint numBuffers)
    {
        Width = width;
        Height = height;
        Format = format;
        NumBuffers = numBuffers;
    }
}