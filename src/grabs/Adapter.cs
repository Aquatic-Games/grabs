namespace grabs;

public readonly struct Adapter
{
    public readonly uint Index;
    
    public readonly string Name;

    public readonly uint DedicatedMemory;

    public readonly AdapterType Type;

    public Adapter(uint index, string name, uint dedicatedMemory, AdapterType type)
    {
        Index = index;
        Name = name;
        DedicatedMemory = dedicatedMemory;
        Type = type;
    }

    public override string ToString()
    {
        return $"""
                {Name}:
                    Index: {Index},
                    DedicatedMemory: {DedicatedMemory / 1024 / 1024}MB,
                    Type: {Type}
                """;
    }
}