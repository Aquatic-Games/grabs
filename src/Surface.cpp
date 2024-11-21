#include "grabs/Surface.h"

#include <stdexcept>

#include "D3D11/D3D11Instance.h"

#ifdef GS_ENABLE_VK
#include "Vk/VulkanInstance.h"
#include "Vk/VulkanSurface.h"
#endif

#ifdef GS_ENABLE_D3D11
#include "D3D11/DXGISurface.h"
#endif

namespace grabs {

    std::unique_ptr<Surface> Surface::Vulkan(Instance* instance, const std::function<void*(void* instance)>& createSurfaceFn) {
#ifdef GS_ENABLE_VK
        auto vkInstance = dynamic_cast<Vk::VulkanInstance*>(instance)->Instance;
        return std::make_unique<Vk::VulkanSurface>(vkInstance, createSurfaceFn);
#else
        throw std::runtime_error("Cannot create Vulkan surface: Vulkan is not enabled.");
#endif
    }

    std::unique_ptr<Surface> Surface::DXGI(size_t hwnd)
    {
#ifdef GS_ENABLE_D3D11
        return std::make_unique<D3D11::DXGISurface>(reinterpret_cast<HWND>(hwnd));
#else
        throw std::runtime_error("Cannot create DXGI surface: D3D is not enabled.");
#endif
    }
}
