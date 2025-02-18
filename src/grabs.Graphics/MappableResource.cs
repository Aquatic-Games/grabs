namespace grabs.Graphics;

public abstract class MappableResource
{
    internal abstract MappedData Map(MapType type);

    internal abstract void Unmap();
}