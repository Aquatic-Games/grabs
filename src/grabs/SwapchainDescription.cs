namespace grabs;

public struct SwapchainDescription
{
    public uint Width;
    
    public uint Height;

    public uint BufferCount;

    public PresentMode PresentMode;

    public SwapchainDescription(uint width, uint height, uint bufferCount, PresentMode presentMode)
    {
        Width = width;
        Height = height;
        BufferCount = bufferCount;
        PresentMode = presentMode;
    }
}