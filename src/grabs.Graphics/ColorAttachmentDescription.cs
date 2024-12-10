using System.Drawing;

namespace grabs.Graphics;

public struct ColorAttachmentDescription
{
    public Texture Texture;

    public Color4 ClearColor;

    public LoadOp LoadOp;

    public ColorAttachmentDescription(Texture texture, Color4 clearColor, LoadOp loadOp = LoadOp.Clear)
    {
        Texture = texture;
        ClearColor = clearColor;
        LoadOp = loadOp;
    }

    public ColorAttachmentDescription(Texture texture, Color clearColor, LoadOp loadOp = LoadOp.Clear)
        : this(texture, (Color4) clearColor, loadOp) { }
}