namespace grabs.Graphics;

/// <summary>
/// A 2-dimensional size.
/// </summary>
public record struct Size2D
{
    /// <summary>
    /// The size's width.
    /// </summary>
    public uint Width;

    /// <summary>
    /// The size's height.
    /// </summary>
    public uint Height;

    public Size2D(uint width, uint height)
    {
        Width = width;
        Height = height;
    }
}