#pragma once

#include "grabs/Swapchain.h"
#include "VulkanSurface.h"
#include "VulkanDevice.h"

namespace grabs::Vk {

    class VulkanSwapchain : public Swapchain {
    public:
        VkDevice Device;
        VkSwapchainKHR Swapchain;

        VulkanSwapchain(VkInstance instance, VkPhysicalDevice physDevice, VulkanDevice* device, const SwapchainDescription& description, VulkanSurface* surface);
        ~VulkanSwapchain() override;
    };

}
