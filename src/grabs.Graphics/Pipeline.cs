namespace grabs.Graphics;

/// <summary>
/// A pipeline describes how the device should process graphics commands.
/// </summary>
public abstract class Pipeline : IDisposable
{
    /// <summary>
    /// Dispose this pipeline.
    /// </summary>
    public abstract void Dispose();
}