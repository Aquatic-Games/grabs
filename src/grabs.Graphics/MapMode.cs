namespace grabs.Graphics;

/// <summary>
/// Defines various ways a resource can be mapped.
/// </summary>
public enum MapMode
{
    /// <summary>
    /// Write to the resource.
    /// </summary>
    Write,
    
    /// <summary>
    /// Read from the resource.
    /// </summary>
    Read,
    
    /// <summary>
    /// Read and write to the resource.
    /// </summary>
    ReadAndWrite
}