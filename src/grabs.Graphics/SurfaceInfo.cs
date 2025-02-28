using System.Runtime.InteropServices;

namespace grabs.Graphics;

/// <summary>
/// A union type containing native pointers to the window manager's display and window, for use in
/// <see cref="Surface"/> creation. 
/// </summary>
public readonly struct SurfaceInfo
{
    /// <summary>
    /// The <see cref="SurfaceType"/>.
    /// </summary>
    public readonly SurfaceType Type;

    /// <summary>
    /// The display pointer.
    /// </summary>
    public readonly SurfaceDisplay Display;

    /// <summary>
    /// The window pointer.
    /// </summary>
    public readonly SurfaceWindow Window;

    /// <summary>
    /// Create a new <see cref="SurfaceInfo"/> with the given type, display and window pointers.
    /// </summary>
    /// <param name="type">The <see cref="SurfaceType"/>.</param>
    /// <param name="display">The display pointer.</param>
    /// <param name="window">The window pointer.</param>
    public SurfaceInfo(SurfaceType type, SurfaceDisplay display, SurfaceWindow window)
    {
        Type = type;
        Display = display;
        Window = window;
    }

    /// <summary>
    /// Create a Win32 surface.
    /// </summary>
    /// <param name="hinstance">A pointer to the HINSTANCE.</param>
    /// <param name="hwnd">A pointer to the HWND.</param>
    /// <returns>A Windows <see cref="SurfaceInfo"/>.</returns>
    public static SurfaceInfo Windows(nint hinstance, nint hwnd)
        => new SurfaceInfo(SurfaceType.Windows, new() { Windows = hinstance }, new() { Windows = hwnd });

    /// <summary>
    /// Create an Xlib surface.
    /// </summary>
    /// <param name="display">A pointer to the display.</param>
    /// <param name="window">The window number.</param>
    /// <returns>An Xlib <see cref="SurfaceInfo"/>.</returns>
    public static SurfaceInfo Xlib(nint display, nint window)
        => new SurfaceInfo(SurfaceType.Xlib, new() { Xlib = display }, new() { Xlib = window });
    
    /// <summary>
    /// Create an XCB surface.
    /// </summary>
    /// <param name="connection">A pointer to the connection.</param>
    /// <param name="window">A pointer to the window.</param>
    /// <returns>An XCB <see cref="SurfaceInfo"/>.</returns>
    public static SurfaceInfo Xcb(nint connection, nint window)
        => new SurfaceInfo(SurfaceType.Xcb, new() { Xcb = connection }, new() { Xcb = window });

    /// <summary>
    /// Create a Wayland surface.
    /// </summary>
    /// <param name="display">A pointer to the display.</param>
    /// <param name="surface">A pointer to the surface.</param>
    /// <returns>A Wayland <see cref="SurfaceInfo"/>.</returns>
    public static SurfaceInfo Wayland(nint display, nint surface)
        => new SurfaceInfo(SurfaceType.Wayland, new() { Wayland = display }, new() { Wayland = surface });
    
    [StructLayout(LayoutKind.Explicit)]
    public struct SurfaceDisplay
    {
        [FieldOffset(0)] public nint Windows;
        [FieldOffset(0)] public nint Xlib;
        [FieldOffset(0)] public nint Xcb;
        [FieldOffset(0)] public nint Wayland;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct SurfaceWindow
    {
        [FieldOffset(0)] public nint Windows;
        [FieldOffset(0)] public nint Xlib;
        [FieldOffset(0)] public nint Xcb;
        [FieldOffset(0)] public nint Wayland;
    }
}