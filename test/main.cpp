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