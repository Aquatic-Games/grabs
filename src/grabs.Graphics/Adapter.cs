namespace grabs.Graphics;

/// <summary>
/// A graphics adapter, usually representing a physical device present in the system.
/// </summary>
public readonly record struct Adapter
{
    /// <summary>
    /// The index, determined by the enumeration order.
    /// </summary>
    public readonly uint Index;
    
    /// <summary>
    /// The adapter's name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The dedicated memory in bytes.
    /// </summary>
    public readonly ulong DedicatedMemory;
}