using System.Runtime.InteropServices;

namespace grabs.Graphics;

public readonly struct SurfaceInfo
{
    public readonly SurfaceType Type;

    public readonly SurfaceDisplay Display;

    public readonly SurfaceWindow Window;

    public SurfaceInfo(SurfaceType type, SurfaceDisplay display, SurfaceWindow window)
    {
        Type = type;
        Display = display;
        Window = window;
    }

    public static SurfaceInfo Windows(nint hinstance, nint hwnd)
        => new SurfaceInfo(SurfaceType.Windows, new() { Windows = hinstance }, new() { Windows = hwnd });

    public static SurfaceInfo Xlib(nint display, nint window)
        => new SurfaceInfo(SurfaceType.Xlib, new() { Xlib = display }, new() { Xlib = window });
    
    public static SurfaceInfo Xcb(nint connection, nint window)
        => new SurfaceInfo(SurfaceType.Xcb, new() { Xcb = connection }, new() { Xcb = window });

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