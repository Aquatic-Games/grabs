using grabs.Core;

namespace grabs;

public record struct ColorAttachmentInfo
{
    public Texture Texture;

    public ColorF ClearColor;

    public LoadOp LoadOp;

    public ColorAttachmentInfo(Texture texture, ColorF clearColor, LoadOp loadOp)
    {
        Texture = texture;
        ClearColor = clearColor;
        LoadOp = loadOp;
    }
}