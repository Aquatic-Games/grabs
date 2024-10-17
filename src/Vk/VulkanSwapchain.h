#pragma once

#include "grabs/Swapchain.h"
#include "VulkanSurface.h"

namespace grabs::Vk {

    class VulkanSwapchain : public Swapchain {
    public:
        VulkanSwapchain(const SwapchainDescription& description, VulkanSurface* surface);
    };

}
