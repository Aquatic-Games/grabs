namespace grabs.Graphics;

/// <summary>
/// Represents a logical device that can have commands issued to it.
/// </summary>
public abstract class Device : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Device"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Dispose of this <see cref="Device"/>.
    /// </summary>
    public abstract void Dispose();
}