namespace grabs.Graphics;

/// <summary>
/// Defines the supported surface types.
/// </summary>
public enum SurfaceType
{
    /// <summary>
    /// A Win32 surface.
    /// </summary>
    Windows,
    
    /// <summary>
    /// An Xlib (X11) surface.
    /// </summary>
    Xlib,
    
    /// <summary>
    /// An XCB surface.
    /// </summary>
    Xcb,
    
    /// <summary>
    /// A Wayland surface.
    /// </summary>
    Wayland
}