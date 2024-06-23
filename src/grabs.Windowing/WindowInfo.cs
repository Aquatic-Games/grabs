using System.Drawing;
using grabs.Graphics;

namespace grabs.Windowing;

public struct WindowInfo
{
    public string Title;

    public Size Size;

    public int? X;
    public int? Y;

    public GraphicsApi? Api;

    public WindowInfo(string title, Size size, int? x = null, int? y = null, GraphicsApi? api = null)
    {
        Title = title;
        Size = size;
        X = x;
        Y = y;
        Api = api;
    }
}