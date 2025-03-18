namespace grabs.Graphics;

/// <summary>
/// Represents a physical graphics device present in a system. 
/// </summary>
public readonly record struct Adapter
{
    public readonly nint Handle;
    
    /// <summary>
    /// The enumerated index of the adapter.
    /// </summary>
    public readonly uint Index;
    
    /// <summary>
    /// The adapter's name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The type of this adapter.
    /// </summary>
    public readonly AdapterType Type;

    /// <summary>
    /// The amount of dedicated memory, in bytes, available to the adapter.
    /// </summary>
    public readonly ulong DedicatedMemory;

    /// <summary>
    /// Features that the adapter supports.
    /// </summary>
    public readonly AdapterFeatures Features;

    /// <summary>
    /// Various limitations of this adapter.
    /// </summary>
    public readonly AdapterLimits Limits;

    /// <summary>
    /// <b>Intended for backend implementations only.</b> Create a new adapter with the given native handle.
    /// </summary>
    /// <param name="handle">The native handle of the adapter. </param>
    /// <param name="index">The enumerated index of the adapter.</param>
    /// <param name="name">The adapter's name.</param>
    /// <param name="type">The type of this adapter.</param>
    /// <param name="dedicatedMemory">The amount of dedicated memory, in bytes, available to the adapter.</param>
    /// <param name="features">Features that the adapter supports.</param>
    /// <param name="limits">Various limitations of this adapter.</param>
    public Adapter(nint handle, uint index, string name, AdapterType type, ulong dedicatedMemory, AdapterFeatures features, AdapterLimits limits)
    {
        Handle = handle;
        Index = index;
        Name = name;
        Type = type;
        DedicatedMemory = dedicatedMemory;
        Features = features;
        Limits = limits;
    }
}