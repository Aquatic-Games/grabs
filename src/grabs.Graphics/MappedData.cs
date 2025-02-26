namespace grabs.Graphics;

/// <summary>
/// Contains data about a mapped resource.
/// </summary>
public readonly struct MappedData
{
    /// <summary>
    /// The pointer to the region of memory where the resource is mapped.
    /// </summary>
    public readonly nint DataPtr;

    /// <summary>
    /// Create a new mapped data.
    /// </summary>
    /// <param name="dataPtr">The pointer to the region of memory where the resource is mapped.</param>
    public MappedData(IntPtr dataPtr)
    {
        DataPtr = dataPtr;
    }
}