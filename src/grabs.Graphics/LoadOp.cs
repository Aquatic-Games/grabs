namespace grabs.Graphics;

/// <summary>
/// An operation that occurs when a color target is loaded.
/// </summary>
public enum LoadOp
{
    /// <summary>
    /// Clear the target with the specified clear color.
    /// </summary>
    Clear,
    
    /// <summary>
    /// Load the current data stored in the target.
    /// </summary>
    Load
}