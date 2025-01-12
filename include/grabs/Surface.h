#pragma once

#ifdef GS_OS_WINDOWS
#include <windows.h>
#endif

#ifdef GS_OS_LINUX
#include <X11/Xlib-xcb.h>
#endif

namespace grabs
{
    enum class SurfaceType
    {
        Windows = 0,
        Xlib = 1,
        XCB = 2,
        Wayland = 3
    };

    union SurfaceDisplay
    {
#ifdef GS_OS_WINDOWS
        HINSTANCE Windows;
#endif
#ifdef GS_OS_LINUX
        Display* Xlib;
        xcb_connection_t* XCB;
        struct wl_display* Wayland;
#endif
        void* _padding;
    };

    union SurfaceWindow
    {
#ifdef GS_OS_WINDOWS
        HWND Windows;
#endif
#ifdef GS_OS_LINUX
        Window Xlib;
        xcb_window_t XCB;
        struct wl_surface* Wayland;
#endif
        void* _padding;
    };

    struct SurfaceDescription
    {
        SurfaceType Type;
        SurfaceDisplay Display;
        SurfaceWindow Window;
    };

    class Surface
    {
    public:
        virtual ~Surface() = default;
    };
}