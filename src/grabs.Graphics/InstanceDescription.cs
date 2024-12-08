namespace grabs.Graphics;

/// <summary>
/// Used to set up a grabs <see cref="Instance"/>.
/// </summary>
public record struct InstanceDescription
{
    /// <summary>
    /// If true, graphics debugging will be enabled.
    /// </summary>
    public bool Debug;

    /// <summary>
    /// Instructs the <see cref="Instance"/> creation to select a specific backend. Use <see cref="Backend.Unknown"/> to
    /// automatically pick a backend.
    /// </summary>
    public Backend BackendHint;

    public InstanceDescription(bool debug, Backend backendHint)
    {
        Debug = debug;
        BackendHint = backendHint;
    }
}