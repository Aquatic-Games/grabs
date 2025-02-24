using grabs.Core;

namespace grabs.Graphics;

/// <summary>
/// Defines a color attachment used in a render pass.
/// </summary>
public record struct ColorAttachmentInfo
{
    /// <summary>
    /// The <see cref="grabs.Graphics.Texture"/> to draw to.
    /// </summary>
    public Texture Texture;

    /// <summary>
    /// The color to clear the <see cref="Texture"/> to.
    /// </summary>
    public ColorF ClearColor;

    /// <summary>
    /// The operation that should be performed when the <see cref="Texture"/> is loaded for drawing.
    /// </summary>
    public LoadOp LoadOp;

    /// <summary>
    /// Define a new color attachment.
    /// </summary>
    /// <param name="texture">The <see cref="grabs.Graphics.Texture"/> to draw to.</param>
    /// <param name="clearColor">The color to clear the <see cref="Texture"/> to.</param>
    /// <param name="loadOp">The operation that should be performed when the <see cref="Texture"/> is loaded for
    /// drawing.</param>
    public ColorAttachmentInfo(Texture texture, ColorF clearColor, LoadOp loadOp = LoadOp.Clear)
    {
        Texture = texture;
        ClearColor = clearColor;
        LoadOp = loadOp;
    }
}