namespace grabs.Graphics;

public interface IBackend : IBackendBase
{
    /// <summary>
    /// Get the name of this backend.
    /// </summary>
    public static abstract string Name { get; }
}