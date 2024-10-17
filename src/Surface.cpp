#include "grabs/Surface.h"

#include "Vk/VulkanInstance.h"
#include "Vk/VulkanSurface.h"

namespace grabs {

    std::unique_ptr<Surface> Surface::Vulkan(Instance* instance, const std::function<void*(void* instance)>& createSurfaceFn) {
        auto vkInstance = dynamic_cast<Vk::VulkanInstance*>(instance)->Instance;

        return std::make_unique<Vk::VulkanSurface>(vkInstance, createSurfaceFn);
    }


}