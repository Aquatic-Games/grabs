using grabs.Core;

namespace grabs.Graphics;

/// <summary>
/// A texture contains image data used for rendering.
/// </summary>
public abstract class Texture : IDisposable
{
    /// <summary>
    /// The size, in pixels, of the texture.
    /// </summary>
    public abstract Size2D Size { get; }
    
    /// <summary>
    /// Dispose of this texture.
    /// </summary>
    public abstract void Dispose();
}