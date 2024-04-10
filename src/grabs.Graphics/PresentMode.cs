namespace grabs.Graphics;

public enum PresentMode
{
    /// <summary>
    /// Present immediately. (VSync off)
    /// </summary>
    Immediate,
    
    /// <summary>
    /// Wait for vertical sync. (VSync on)
    /// </summary>
    VerticalSync,
    
    /// <summary>
    /// Adaptive VSync. Not supported on all platforms. If unsupported, <see cref="VerticalSync"/> will be used instead.
    /// </summary>
    AdaptiveSync
}