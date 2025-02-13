using grabs.Core;

namespace grabs.Graphics;

public record struct ColorAttachmentInfo
{
    public Texture Texture;

    public ColorF ClearColor;

    public LoadOp LoadOp;

    public ColorAttachmentInfo(Texture texture, ColorF clearColor, LoadOp loadOp = LoadOp.Clear)
    {
        Texture = texture;
        ClearColor = clearColor;
        LoadOp = loadOp;
    }
}