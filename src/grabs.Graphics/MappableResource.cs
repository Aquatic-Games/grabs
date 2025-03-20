namespace grabs.Graphics;

/// <summary>
/// Represents a device resource that can be mapped into CPU accessible memory.
/// </summary>
public abstract class MappableResource
{
    /// <summary>
    /// Map the resource into memory.
    /// </summary>
    /// <returns>Data about the mapped resource.</returns>
    protected internal abstract MappedData Map();

    /// <summary>
    /// Unmap the resource from memory.
    /// </summary>
    protected internal abstract void Unmap();
}