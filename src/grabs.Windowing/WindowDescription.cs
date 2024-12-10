namespace grabs.Windowing;

public struct WindowDescription
{
    public uint Width;

    public uint Height;

    public string Title;

    public WindowDescription(uint width, uint height, string title)
    {
        Width = width;
        Height = height;
        Title = title;
    }
}