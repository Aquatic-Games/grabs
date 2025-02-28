namespace grabs.Graphics;

/// <summary>
/// A viewport defines the mapping between the device and window coordinates.
/// </summary>
public record struct Viewport
{
    /// <summary>
    /// The X position, in pixels.
    /// </summary>
    public float X;

    /// <summary>
    /// The Y position, in pixels.
    /// </summary>
    public float Y;

    /// <summary>
    /// The width, in pixels.
    /// </summary>
    public float Width;

    /// <summary>
    /// The height, in pixels.
    /// </summary>
    public float Height;

    /// <summary>
    /// The minimum depth, in device coordinates.
    /// </summary>
    public float MinDepth;

    /// <summary>
    /// The maximum depth, in device coordinates.
    /// </summary>
    public float MaxDepth;

    /// <summary>
    /// Create a new <see cref="Viewport"/>.
    /// </summary>
    /// <param name="x">The X position, in pixels.</param>
    /// <param name="y">The Y position, in pixels.</param>
    /// <param name="width">The width, in pixels.</param>
    /// <param name="height">The height, in pixels.</param>
    /// <param name="minDepth">The minimum depth, in device coordinates.</param>
    /// <param name="maxDepth">The maximum depth, in device coordinates.</param>
    public Viewport(float x, float y, float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
    }
}