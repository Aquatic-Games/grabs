namespace grabs;

public readonly struct Adapter
{
    public readonly uint Index;
    
    public readonly string Name;

    public Adapter(uint index, string name)
    {
        Index = index;
        Name = name;
    }
}