namespace grabs;

public readonly record struct Adapter
{
    public readonly uint Index;
    
    public readonly string Name;

    public readonly AdapterType Type;

    public readonly ulong DedicatedMemory;

    public Adapter(uint index, string name, AdapterType type, ulong dedicatedMemory)
    {
        Index = index;
        Name = name;
        Type = type;
        DedicatedMemory = dedicatedMemory;
    }
}