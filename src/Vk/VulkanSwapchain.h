#pragma once

#include "grabs/Surface.h"
#include "grabs/Swapchain.h"

namespace grabs::Vk {

    class VulkanSwapchain : public Swapchain {
    public:
        VulkanSwapchain(const SwapchainDescription& description, VulkanSurface* surface);
    };

}
