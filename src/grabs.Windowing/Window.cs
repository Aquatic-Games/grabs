using grabs.Core;
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