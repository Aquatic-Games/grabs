namespace grabs.Graphics;

public readonly record struct Adapter(nint Handle, uint Index, string Name, AdapterType Type, ulong DedicatedMemory);