using grabs.Graphics;

namespace grabs.Windowing;

public struct WindowInfo
{
    public string Title;

    public uint Width;
    public uint Height;

    public int? X;
    public int? Y;

    public GraphicsApi? Api;

    public WindowInfo(string title, uint width, uint height, int? x = null, int? y = null, GraphicsApi? api = null)
    {
        Title = title;
        Width = width;
        Height = height;
        X = x;
        Y = y;
        Api = api;
    }
}