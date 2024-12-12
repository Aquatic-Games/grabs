namespace grabs.Windowing;

public struct WindowDescription
{
    public uint Width;

    public uint Height;

    public string Title;

    public int? X;

    public int? Y;

    public WindowDescription(uint width, uint height, string title, int? x = null, int? y = null)
    {
        Width = width;
        Height = height;
        Title = title;
        X = x;
        Y = y;
    }
}