using grabs.Core;

namespace grabs.Windowing;

public record struct WindowInfo
{
    public Size2D Size;

    public string Title;

    public WindowInfo(Size2D size, string title)
    {
        Size = size;
        Title = title;
    }
}