#pragma once

#include <vector>

#include "grabs/Swapchain.h"
#include "VulkanSurface.h"
#include "VulkanDevice.h"
#include "VulkanTexture.h"

namespace grabs::Vk
{
    class VulkanSwapchain final : public Swapchain
    {
    public:
        VkDevice Device{};
        VkSwapchainKHR Swapchain{};

        std::vector<std::unique_ptr<VulkanTexture>> SwapchainTextures;

        VulkanSwapchain(VkInstance instance, VkPhysicalDevice physDevice, VulkanDevice* device, const SwapchainDescription& description, VulkanSurface* surface);
        ~VulkanSwapchain() override;

        Texture* GetNextTexture() override;

        void Present() override;
    };

}
