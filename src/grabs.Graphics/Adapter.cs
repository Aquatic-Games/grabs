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

    /// <summary>
    /// The adapter's type.
    /// </summary>
    /// <remarks>In some backends, an integrated GPU may show up as <see cref="AdapterType.Dedicated"/>, as these
    /// backends do not differentiate between the two.</remarks>
    public readonly AdapterType Type;

    public Adapter(uint index, string name, ulong dedicatedMemory, AdapterType type)
    {
        Index = index;
        Name = name;
        DedicatedMemory = dedicatedMemory;
        Type = type;
    }
}