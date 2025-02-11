namespace grabs;

public readonly record struct Adapter
{
    internal readonly nint Handle;
    
    public readonly uint Index;
    
    public readonly string Name;

    public readonly AdapterType Type;

    public readonly ulong DedicatedMemory;

    public Adapter(nint handle, uint index, string name, AdapterType type, ulong dedicatedMemory)
    {
        Handle = handle;
        Index = index;
        Name = name;
        Type = type;
        DedicatedMemory = dedicatedMemory;
    }
}