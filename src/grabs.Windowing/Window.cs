using SDL;
using static SDL.SDL3;

namespace grabs.Windowing;

public unsafe class Window : IDisposable
{
    private readonly SDL_Window* _window;
    
    public Window(in WindowInfo info)
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS))
            
    }
    
    public void Dispose()
    {
        
    }
}