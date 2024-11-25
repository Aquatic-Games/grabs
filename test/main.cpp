#include <grabs/Instance.h>
#include <SDL2/SDL.h>
#include <SDL2/SDL_vulkan.h>
#include <SDL2/SDL_syswm.h>
#include <vector>
#include <string>

#include <iostream>

using namespace grabs;

std::vector<uint8_t> ReadFile(const std::string& path)
{
    size_t fileSize;
    auto data = SDL_LoadFile(path.c_str(), &fileSize);

    if (!data)
        throw std::runtime_error("Failed to load file.");

    std::vector vecData(static_cast<uint8_t*>(data), static_cast<uint8_t*>(data) + fileSize);

    SDL_free(data);

    return vecData;
}

int main(int argc, char* argv[]) {
    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS) < 0) {
        std::cout << "Failed to initialize SDL: " << SDL_GetError() << std::endl;
        return 1;
    }

    constexpr int width = 1280;
    constexpr int height = 720;

    SDL_Window* window = SDL_CreateWindow("Test", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, SDL_WINDOW_VULKAN);

    if (!window) {
        std::cout << "Failed to create window: " << SDL_GetError() << std::endl;
        return 1;
    }

    InstanceInfo info {
        .Debug = true,
        .BackendHint = Backend::Unknown,
        .GetInstanceExtensions = [window]() {
            uint32_t numExtensions;
            SDL_Vulkan_GetInstanceExtensions(window, &numExtensions, nullptr);
            std::vector<const char*> extensions(numExtensions);
            SDL_Vulkan_GetInstanceExtensions(window, &numExtensions, extensions.data());
            return extensions;
        }
    };

    auto instance = Instance::Create(info);
    std::cout << "Backend: " << BackendToFriendlyString(instance->Backend()) << std::endl;

    auto adapters = instance->EnumerateAdapters();
    for (const auto& adapter : adapters) {
        std::cout << adapter.Name << " - " << adapter.Memory / 1024 / 1024 << "MB" << std::endl;
    }

    std::unique_ptr<Surface> surface = nullptr;
    switch (instance->Backend())
    {
        case Backend::Vulkan:
        {
            surface = Surface::Vulkan(instance.get(), [window](void* vkInstance) {
                VkSurfaceKHR surface;
                SDL_Vulkan_CreateSurface(window, static_cast<VkInstance>(vkInstance), &surface);
                return surface;
            });
            break;
        }
        case Backend::D3D11:
        {
            SDL_SysWMinfo info;
            SDL_VERSION(&info.version);
            SDL_GetWindowWMInfo(window, &info);

            surface = Surface::DXGI(reinterpret_cast<size_t>(info.info.win.window));
            break;
        }
        default:
            throw std::runtime_error("No backend");
    }

    auto device = instance->CreateDevice(surface.get());

    SwapchainDescription swapchainDesc {
        .Size = { width, height },
        .Format = Format::B8G8R8A8_UNorm,
        .NumBuffers = 2,
        .PresentMode = PresentMode::Fifo
    };

    auto swapchain = device->CreateSwapchain(swapchainDesc, surface.get());

    auto cl = device->CreateCommandList();

    constexpr float vertices[]
    {
        -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
         0.5f,  0.5f,    0.0f, 1.0f, 0.0f,
         0.5f, -0.5f,    0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f,    0.0f, 0.0f, 0.0f
    };
    auto vertexBuffer = device->CreateBuffer({ .Type = BufferType::Vertex, .Size = sizeof(vertices), .Dynamic = false }, (void*) vertices);

    constexpr uint16_t indices[]
    {
        0, 1, 3,
        1, 2, 3
    };
    auto indexBuffer = device->CreateBuffer({ .Type = BufferType::Index, .Size = sizeof(indices), .Dynamic = false }, (void*) indices);

    auto vertSpv = ReadFile("Test_v.spv");
    auto vertexShader = device->CreateShaderModule({ .Stage = ShaderStage::Vertex, .Spirv = vertSpv, .EntryPoint = "VSMain" });

    auto pixlSpv = ReadFile("Test_p.spv");
    auto pixelShader = device->CreateShaderModule({ .Stage = ShaderStage::Pixel, .Spirv = pixlSpv, .EntryPoint = "PSMain" });

    bool alive = true;
    while (alive) {
        SDL_Event event;
        while (SDL_PollEvent(&event)) {
            switch (event.type) {
                case SDL_WINDOWEVENT: {
                    switch (event.window.event) {
                        case SDL_WINDOWEVENT_CLOSE: {
                            alive = false;
                            break;
                        }
                    }

                    break;
                }
            }
        }

        auto texture = swapchain->GetNextTexture();
        cl->Begin();

        RenderPassDescription renderPass
        {
            .Texture = texture,
            .ClearColor = { 1.0f, 0.5f, 0.25f, 1.0f }
        };
        cl->BeginRenderPass(renderPass);
        cl->EndRenderPass();

        cl->End();
        device->SubmitCommandList(cl.get());

        swapchain->Present();
    }

    SDL_DestroyWindow(window);
    SDL_Quit();

    return 0;
}