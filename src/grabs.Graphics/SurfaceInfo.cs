namespace grabs.Graphics;

public struct SurfaceInfo(SurfaceType type, nint display, nint window)
{
    public SurfaceType Type = type;

    public nint Display = display;

    public nint Window = window;

    public static SurfaceInfo Windows(nint hinstance, nint hwnd)
        => new SurfaceInfo(SurfaceType.Windows, hinstance, hwnd);

    public static SurfaceInfo Xlib(nint display, nint window)
        => new SurfaceInfo(SurfaceType.Xlib, display, window);

    public static SurfaceInfo Xcb(nint connection, nint window)
        => new SurfaceInfo(SurfaceType.Xcb, connection, window);

    public static SurfaceInfo Wayland(nint display, nint surface)
        => new SurfaceInfo(SurfaceType.Wayland, display, surface);
}