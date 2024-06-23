using System;
using grabs.Graphics;
using grabs.Windowing.Events;
using Silk.NET.SDL;
using QuitEvent = grabs.Windowing.Events.QuitEvent;
using SdlEventType = Silk.NET.SDL.EventType;
using SdlWindow = Silk.NET.SDL.Window;

namespace grabs.Windowing;

public sealed unsafe class Window : IDisposable
{
    private Sdl _sdl;
    private SdlWindow* _handle;
    private void* _glContext;

    private GraphicsApi _api;
    
    public Window(WindowInfo info)
    {
        _sdl = Sdl.GetApi();
        _api = info.Api ?? GraphicsApi.OpenGL; // TODO: Temporary

        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {_sdl.GetErrorS()}");

        // Wayland ftw...
        _sdl.SetHint("SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR", "0");

        WindowFlags flags = WindowFlags.Shown;

        switch (_api)
        {
            case GraphicsApi.D3D11:
                break;
            case GraphicsApi.OpenGL:
                flags |= WindowFlags.Opengl;
                
                _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 4);
                _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);
                _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) GLprofile.Core);
                break;
            case GraphicsApi.OpenGLES:
                flags |= WindowFlags.Opengl;

                _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 3);
                _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 0);
                _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) GLprofile.ES);
                break;
            case GraphicsApi.Vulkan:
                throw new NotSupportedException("Vulkan is not yet supported by the windowing framework.");
            default:
                throw new ArgumentOutOfRangeException(nameof(_api), _api, null);
        }

        _handle = _sdl.CreateWindow(info.Title, info.X ?? Sdl.WindowposCentered, info.Y ?? Sdl.WindowposCentered,
            (int) info.Width, (int) info.Height, (uint) flags);

        if (_handle == null)
            throw new Exception($"Failed to create SDL window: {_sdl.GetErrorS()}");

        if (_api is GraphicsApi.OpenGL or GraphicsApi.OpenGLES)
        {
            _glContext = _sdl.GLCreateContext(_handle);
            _sdl.GLMakeCurrent(_handle, _glContext);
        }
    }

    public bool PollEvent(out IWindowEvent @event)
    {
        @event = null;
        
        Event winEvent;
        while (_sdl.PollEvent(&winEvent) != 0)
        {
            switch ((SdlEventType) winEvent.Type)
            {
                case SdlEventType.Windowevent:
                {
                    switch ((WindowEventID) winEvent.Window.Event)
                    {
                        case WindowEventID.Close:
                            @event = new QuitEvent();
                            return true;
                        
                        default:
                            continue;
                    }
                }
                
                default:
                    continue;
            }
        }

        return false;
    }
    
    public void Dispose()
    {
        if (_glContext != null)
            _sdl.GLDeleteContext(_glContext);
        
        _sdl.DestroyWindow(_handle);
        _sdl.Quit();
        _sdl.Dispose();
    }
}