namespace grabs.Graphics;

/// <summary>
/// Describes a constant that can be pushed to a shader.
/// </summary>
public record struct ConstantLayout
{
    /// <summary>
    /// The shader stage(s) that this constant is defined in.
    /// </summary>
    public ShaderStage Stages;

    /// <summary>
    /// The offset, in bytes.
    /// </summary>
    public uint Offset;

    /// <summary>
    /// The size, in bytes.
    /// </summary>
    public uint Size;

    /// <summary>
    /// Create a new <see cref="ConstantLayout"/>.
    /// </summary>
    /// <param name="stages">The shader stage(s) that this constant is defined in.</param>
    /// <param name="offset">The offset, in bytes.</param>
    /// <param name="size">The size, in bytes.</param>
    public ConstantLayout(ShaderStage stages, uint offset, uint size)
    {
        Stages = stages;
        Offset = offset;
        Size = size;
    }
}