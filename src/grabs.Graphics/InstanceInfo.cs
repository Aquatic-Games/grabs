namespace grabs.Graphics;

/// <summary>
/// Describes how an <see cref="Instance"/> should be created.
/// </summary>
public struct InstanceInfo
{
    /// <summary>
    /// The name of your application. This may be used in some backends to describe the application to the driver.
    /// </summary>
    public string AppName;

    /// <summary>
    /// Enable debugging. This will enable validation layers.
    /// </summary>
    public bool Debug;

    /// <summary>
    /// Create a new <see cref="InstanceInfo"/>.
    /// </summary>
    /// <param name="appName">The name of your application. This may be used in some backends to describe the
    /// application to the driver.</param>
    /// <param name="debug">Enable debugging. This will enable validation layers.</param>
    public InstanceInfo(string appName, bool debug = false)
    {
        AppName = appName;
        Debug = debug;
    }
}