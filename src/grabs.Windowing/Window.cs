using grabs.Core;
using grabs.Graphics;
using grabs.Windowing.Events;
using SDL;
using static SDL.SDL3;

namespace grabs.Windowing;

public unsafe class Window : IDisposable
{
    private readonly SDL_Window* _window;

    public nint Handle => (nint) _window;

    public Size2D Size
    {
        get
        {
            int w, h;
            SDL_GetWindowSize(_window, &w, &h);

            return new Size2D((uint) w, (uint) h);
        }
        
        set => SDL_SetWindowSize(_window, (int) value.Width, (int) value.Width);
    }
    
    public Window(in WindowInfo info)
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS))
            throw new Exception($"Failed to initialize SDL: {SDL_GetError()}");

        _window = SDL_CreateWindow(info.Title, (int) info.Size.Width, (int) info.Size.Height, 0);

        if (_window == null)
            throw new Exception($"Failed to create window: {SDL_GetError()}");
    }

    public Surface CreateSurface(Instance instance)
    {
        SDL_PropertiesID props = SDL_GetWindowProperties(_window);
        
        SurfaceInfo info;

        if (OperatingSystem.IsWindows())
        {
            nint hinstance = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_INSTANCE_POINTER, IntPtr.Zero);
            nint hwnd = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WIN32_HWND_POINTER, IntPtr.Zero);
            
            info = SurfaceInfo.Windows(hinstance, hwnd);
        }
        else if (OperatingSystem.IsLinux())
        {
            string driver = SDL_GetCurrentVideoDriver();

            switch (driver)
            {
                case "wayland":
                {
                    nint display = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WAYLAND_DISPLAY_POINTER, IntPtr.Zero);
                    nint surface = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_WAYLAND_SURFACE_POINTER, IntPtr.Zero);

                    info = SurfaceInfo.Wayland(display, surface);
                    break;
                }
                case "x11":
                {
                    nint display = SDL_GetPointerProperty(props, SDL_PROP_WINDOW_X11_DISPLAY_POINTER, IntPtr.Zero);
                    long window = SDL_GetNumberProperty(props, SDL_PROP_WINDOW_X11_WINDOW_NUMBER, 0);

                    info = SurfaceInfo.Xlib(display, (nint) window);
                    break;
                }
                default:
                    throw new NotSupportedException(driver);
            }
        }
        else
            throw new PlatformNotSupportedException();

        return instance.CreateSurface(in info);
    }

    public bool PollEvent(out Event @event)
    {
        SDL_Event sdlEvent;
        while (SDL_PollEvent(&sdlEvent))
        {
            switch (sdlEvent.Type)
            {
                case SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                    @event = new Event(EventType.Close);
                    return true;
                
                default:
                    continue;
            }
        }

        @event = new Event(EventType.None);
        return false;
    }
    
    public void Dispose()
    {
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }
}