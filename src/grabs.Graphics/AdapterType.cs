namespace grabs.Graphics;

/// <summary>
/// Represents various types a graphics <see cref="Adapter"/> can be.
/// </summary>
public enum AdapterType
{
    /// <summary>
    /// Used for adapter types that are unknown.
    /// </summary>
    Other,
    
    /// <summary>
    /// The adapter is using software rendering.
    /// </summary>
    Software,
    
    /// <summary>
    /// The adapter is an integrated graphics device, for example the integrated graphics on a CPU.
    /// </summary>
    Integrated,
    
    /// <summary>
    /// The adapter is a dedicated graphics device, for example an installed graphics card.
    /// </summary>
    Dedicated
}