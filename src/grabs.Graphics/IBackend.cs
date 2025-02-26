namespace grabs.Graphics;

/// <summary>
/// Represents a registerable and creatable backend.
/// </summary>
public interface IBackend : IBackendBase
{
    /// <summary>
    /// Get the name of the backend.
    /// </summary>
    public static abstract string Name { get; }
}