#include "grabs/Surface.h"

#include <stdexcept>

#ifdef GS_ENABLE_VK
#include "Vk/VulkanInstance.h"
#include "Vk/VulkanSurface.h"
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


}