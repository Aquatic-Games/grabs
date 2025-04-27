namespace grabs.Graphics;

/// <summary>
/// Describes an <see cref="Instance"/>.
/// </summary>
/// <param name="appName">The application name.</param>
/// <param name="debug">If the instance should be created with debugging features enabled.</param>
public struct InstanceInfo(string appName, bool debug = false)
{
    /// <summary>
    /// The application name.
    /// </summary>
    public string AppName = appName;

    /// <summary>
    /// If the instance should be created with debugging features enabled.
    /// </summary>
    public bool Debug = debug;
}