namespace grabs;

public struct SwapchainDescription
{
    public uint Width;
    
    public uint Height;

    public uint BufferCount;

    public SwapchainDescription(uint width, uint height, uint bufferCount)
    {
        Width = width;
        Height = height;
        BufferCount = bufferCount;
    }
}