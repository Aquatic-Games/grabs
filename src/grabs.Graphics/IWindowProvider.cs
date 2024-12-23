namespace grabs.Graphics;

/// <summary>
/// Provides various functions for grabs instances to use, mostly during creation.
/// Usually this should be implemented in Window classes.
/// </summary>
public interface IWindowProvider
{
    public string[] GetVulkanInstanceExtensions();

    public nint GetGLProcAddress(string name);
}