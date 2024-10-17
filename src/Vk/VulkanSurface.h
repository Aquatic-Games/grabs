#pragma once

#include <vulkan/vulkan.h>

#include "grabs/Surface.h"

namespace grabs::Vk {

    class VulkanSurface : public Surface {
    public:
        VkInstance Instance;
        VkSurfaceKHR Surface;

        explicit VulkanSurface(VkInstance instance, const std::function<void*(void* instance)>& createSurfaceFn) {
            Instance = instance;
            Surface = static_cast<VkSurfaceKHR>(createSurfaceFn(instance));
        }

        ~VulkanSurface() override {
            vkDestroySurfaceKHR(Instance, Surface, nullptr);
        }
    };

}
