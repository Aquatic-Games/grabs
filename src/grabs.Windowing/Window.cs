using System.Diagnostics.CodeAnalysis;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Windowing.Events;
using Silk.NET.SDL;
using EventType = Silk.NET.SDL.EventType;
using QuitEvent = grabs.Windowing.Events.QuitEvent;
using SdlWindow = Silk.NET.SDL.Window;
using Surface = grabs.Graphics.Surface;

namespace grabs.Windowing;

public unsafe class Window : IWindowProvider, IDisposable
{
    private SdlWindow* _window;
    
    private Window(SdlWindow* window)
    {
        _window = window;
    }

    public Surface CreateSurface(Instance instance)
    {
        switch (instance.Backend)
        {
            case Backend.Unknown:
            case Backend.Vulkan:
                throw new NotImplementedException();

            case Backend.D3D11:
            {
                SysWMInfo info = new SysWMInfo();
                _sdl.GetWindowWMInfo(_window, &info);

                return new D3D11Surface(info.Info.Win.Hwnd);
            }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool PollEvent([MaybeNullWhen(false)] out IWindowEvent winEvent)
    {
        Event sdlEvent;
        while (_sdl.PollEvent(&sdlEvent) != 0)
        {
            switch ((EventType) sdlEvent.Type)
            {
                case EventType.Windowevent:
                {
                    switch ((WindowEventID) sdlEvent.Window.Event)
                    {
                        case WindowEventID.Close:
                        {
                            winEvent = new QuitEvent();
                            return true;
                        }
                    }
                    
                    break;
                }
                
                default:
                    continue;
            }
        }

        winEvent = default;
        return false;
    }
    
    public string[] GetInstanceExtensions()
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
    }
        
    private static Sdl _sdl;

    static Window()
    {
        _sdl = Sdl.GetApi();
    }
    
    public static Window Create(in WindowDescription description)
    {
        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {_sdl.GetErrorS()}");

        SdlWindow* window = _sdl.CreateWindow(description.Title, Sdl.WindowposCentered, Sdl.WindowposCentered,
            (int) description.Width, (int) description.Height, 0);

        if (window == null)
        {
            _sdl.Quit();
            throw new Exception($"Failed to create SDL window: {_sdl.GetErrorS()}");
        }

        return new Window(window);
    }
}