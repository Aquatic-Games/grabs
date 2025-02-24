namespace grabs.Graphics;

/// <summary>
/// Contains information about a shader input element, used during pipeline creation.
/// </summary>
public record struct InputElement
{
    /// <summary>
    /// The <see cref="grabs.Graphics.Format"/> of the element.
    /// </summary>
    public Format Format;

    /// <summary>
    /// The offset, in bytes, of the element.
    /// </summary>
    public uint Offset;

    /// <summary>
    /// The vertex buffer slot of the element.
    /// </summary>
    public uint Slot;

    /// <summary>
    /// Create an input element description.
    /// </summary>
    /// <param name="format">The <see cref="grabs.Graphics.Format"/> of the element.</param>
    /// <param name="offset">The offset, in bytes, of the element.</param>
    /// <param name="slot">The vertex buffer slot of the element.</param>
    public InputElement(Format format, uint offset, uint slot)
    {
        Format = format;
        Offset = offset;
        Slot = slot;
    }
}