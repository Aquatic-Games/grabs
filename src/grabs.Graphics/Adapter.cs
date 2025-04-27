namespace grabs.Graphics;

public readonly struct Adapter(nint handle, uint index, string name, AdapterType type, ulong dedicatedMemory)
    : IEquatable<Adapter>, IFormattable
{
    public readonly nint Handle = handle;

    public readonly uint Index = index;

    public readonly string Name = name;

    public readonly AdapterType Type = type;

    public readonly ulong DedicatedMemory = dedicatedMemory;

    public bool Equals(Adapter other)
    {
        return Handle == other.Handle;
    }

    public override bool Equals(object? obj)
    {
        return obj is Adapter other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return $"{Name}:\n    Type: {Type}\n    Dedicated Memory: {DedicatedMemory / 1024 / 1024}MiB";
    }

    public static bool operator ==(Adapter left, Adapter right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Adapter left, Adapter right)
    {
        return !left.Equals(right);
    }
}