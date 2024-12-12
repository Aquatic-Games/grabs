namespace grabs.Graphics;

/// <summary>
/// Represents various types of adapters that may be present in a system.
/// </summary>
public enum AdapterType
{
    /// <summary>
    /// Dedicated GPU. Typically the most powerful GPU present in a system, and should be preferred over other options.
    /// </summary>
    Dedicated,
    
    /// <summary>
    /// Integrated GPU. Typically present in the CPU.
    /// </summary>
    Integrated,
    
    /// <summary>
    /// Software GPU. This may be provided by drivers present on the system.
    /// </summary>
    Software,
    
    /// <summary>
    /// It's not known what type this GPU is.
    /// </summary>
    Unknown
}