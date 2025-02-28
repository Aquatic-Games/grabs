namespace grabs.Graphics;

/// <summary>
/// Represents a shader that can be used during pipeline creation.
/// </summary>
public abstract class ShaderModule : IDisposable
{
    /// <summary>
    /// Dispose of this shader module.
    /// </summary>
    public abstract void Dispose();
}