#include <iostream>

#include <SDL3/SDL.h>
#include <SDL3/SDL_main.h>
#include <grabs/Instance.h>

int main(int argc, char* argv[])
{
    if (!SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS))
    {
        std::cout << "Failed to initialize SDL: " << SDL_GetError() << std::endl;
        return -1;
    }

    SDL_Window* window = SDL_CreateWindow("GRABS test with SDL3", 1280, 720, 0);
    if (!window)
    {
        std::cout << "Failed to create window: " << SDL_GetError() << std::endl;
        return -1;
    }

    grabs::InstanceInfo info
    {
        .Debug = true,
        .BackendHint = grabs::Backend::Unknown,
        .AppName = "GRABS test"
    };

    auto instance = grabs::Instance::Create(info);
    for (const auto& adapter : instance->EnumerateAdapters())
        std::cout << adapter.Name << std::endl;

    grabs::SurfaceDescription surfaceDesc{};

#ifdef GS_OS_WINDOWS
    HINSTANCE hinstance = (HINSTANCE) SDL_GetPointerProperty(SDL_GetWindowProperties(window), SDL_PROP_WINDOW_WIN32_INSTANCE_POINTER, nullptr);
    HWND hwnd = (HWND) SDL_GetPointerProperty(SDL_GetWindowProperties(window), SDL_PROP_WINDOW_WIN32_HWND_POINTER, nullptr);

    surfaceDesc.Display.Windows = hinstance;
    surfaceDesc.Window.Windows = hwnd;
#endif
#ifdef GS_OS_LINUX
    if (SDL_strcmp(SDL_GetCurrentVideoDriver(), "x11") == 0)
    {
        Display* display = (Display*) SDL_GetPointerProperty(SDL_GetWindowProperties(window), SDL_PROP_WINDOW_X11_DISPLAY_POINTER, nullptr);
        Window xwindow = (Window) SDL_GetNumberProperty(SDL_GetWindowProperties(window), SDL_PROP_WINDOW_X11_WINDOW_NUMBER, 0);

        surfaceDesc.Type = grabs::SurfaceType::Xlib;
        surfaceDesc.Display.Xlib = display;
        surfaceDesc.Window.Xlib = xwindow;
    }
#endif

    auto surface = instance->CreateSurface(surfaceDesc);

    bool alive = true;
    while (alive)
    {
        SDL_Event event;
        while (SDL_PollEvent(&event))
        {
            switch (event.type)
            {
                case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                    alive = false;
                    break;
            }
        }
    }

    SDL_DestroyWindow(window);
    SDL_Quit();

    return 0;
}