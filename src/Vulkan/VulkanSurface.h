#pragma once

#include <vulkan/vulkan.h>

#include "grabs/Surface.h"

namespace grabs::Vulkan
{
    class VulkanSurface : public Surface
    {
    public:
        VkInstance Instance{};
        VkSurfaceKHR Surface{};

        VulkanSurface(VkInstance instance, const SurfaceDescription& description);
        ~VulkanSurface() override;
    };
}
