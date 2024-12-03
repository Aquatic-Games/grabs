namespace grabs.Graphics;

/// <summary>
/// An instance contains the base methods to create and manage <see cref="Device"/>s.
/// </summary>
public abstract class Instance : IDisposable
{
    /// <summary>
    /// Check to see if this <see cref="Instance"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; }

    /// <summary>
    /// Enumerate through all supported <see cref="Adapter"/>s available in the system.
    /// </summary>
    /// <returns>A list of <see cref="Adapter"/>s.</returns>
    public abstract Adapter[] EnumerateAdapters();
    
    public abstract void Dispose();
}