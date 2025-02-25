namespace grabs.Graphics;

public abstract class MappableResource
{
    protected internal abstract MappedData Map(MapType type);

    protected internal abstract void Unmap();
}