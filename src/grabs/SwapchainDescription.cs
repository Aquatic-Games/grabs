namespace grabs;

public struct SwapchainDescription
{
    public uint Width;
    
    public uint Height;

    public Format Format;

    public uint BufferCount;

    public PresentMode PresentMode;

    public SwapchainDescription(uint width, uint height, Format format = Format.B8G8R8A8_UNorm, uint bufferCount = 2, PresentMode presentMode = PresentMode.Immediate)
    {
        Width = width;
        Height = height;
        Format = format;
        BufferCount = bufferCount;
        PresentMode = presentMode;
    }

    public override string ToString()
    {
        return $"""
                Width: {Width},
                Height: {Height},
                Format: {Format},
                BufferCount: {BufferCount},
                PresentMode: {PresentMode}
                """;
    }
}