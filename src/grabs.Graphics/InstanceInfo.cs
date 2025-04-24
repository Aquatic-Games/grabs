namespace grabs.Graphics;

/// <summary>
/// Describes an <see cref="Instance"/>.
/// </summary>
/// <param name="AppName">The application name.</param>
/// <param name="Debug">If the instance should be created with debugging features enabled.</param>
public record struct InstanceInfo(string AppName, bool Debug = false);