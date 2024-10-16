#include <grabs/Instance.h>
#include <SDL2/SDL.h>
#include <SDL2/SDL_vulkan.h>

#include <iostream>

using namespace grabs;

int main(int argc, char* argv[]) {
    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS) < 0) {
        std::cout << "Failed to initialize SDL: " << SDL_GetError() << std::endl;
        return 1;
    }

    const int width = 1280;
    const int height = 720;

    SDL_Window* window = SDL_CreateWindow("Test", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, SDL_WINDOW_VULKAN);

    if (!window) {
        std::cout << "Failed to create window: " << SDL_GetError() << std::endl;
        return 1;
    }

    InstanceInfo info {
        .Debug = true,
        .GetInstanceExtensions = [window]() {
            uint32_t numExtensions;
            SDL_Vulkan_GetInstanceExtensions(window, &numExtensions, nullptr);
            std::vector<const char*> extensions(numExtensions);
            SDL_Vulkan_GetInstanceExtensions(window, &numExtensions, extensions.data());
            return extensions;
        },
        .CreateSurface = [window](void* instance) {
            VkSurfaceKHR surface;
            SDL_Vulkan_CreateSurface(window, static_cast<VkInstance>(instance), &surface);
            return surface;
        }
    };

    auto instance = Instance::Create(info);

    auto adapters = instance->EnumerateAdapters();
    for (const auto& adapter : adapters) {
        std::cout << adapter.Name << " - " << adapter.Memory / 1024 / 1024 << "MB" << std::endl;
    }

    SDL_DestroyWindow(window);
    SDL_Quit();

    return 0;
}