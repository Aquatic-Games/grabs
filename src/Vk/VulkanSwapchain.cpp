#include "VulkanSwapchain.h"

#include <algorithm>

#include "VkUtils.h"

namespace grabs::Vk {
    VulkanSwapchain::VulkanSwapchain(VkInstance instance, VkPhysicalDevice physDevice, VulkanDevice* device, const SwapchainDescription& description, VulkanSurface* surface) {
        Device = device->Device;

        VkSurfaceCapabilitiesKHR surfaceCapabilities;
        vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physDevice, surface->Surface, &surfaceCapabilities);

        uint32_t minImages = std::clamp(description.NumBuffers, surfaceCapabilities.minImageCount, surfaceCapabilities.maxImageCount);

        VkFormat format = Utils::FormatToVk(description.Format);
        VkPresentModeKHR presentMode = Utils::PresentModeToVk(description.PresentMode);

        VkExtent2D extent {
            description.Size.Width,
            description.Size.Height
        };

        VkSwapchainCreateInfoKHR swapchainCreateInfo {
            .sType = VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR,
            .surface = surface->Surface,
            .minImageCount = minImages,
            .imageFormat = format,
            .imageColorSpace = VK_COLOR_SPACE_SRGB_NONLINEAR_KHR,
            .imageExtent = extent,
            .imageArrayLayers = 1,
            .imageUsage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
            .preTransform = surfaceCapabilities.currentTransform,
            .compositeAlpha = VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR,
            .presentMode = presentMode,
            .clipped = VK_FALSE,
            .oldSwapchain = nullptr
        };

        CHECK_RESULT(vkCreateSwapchainKHR(Device, &swapchainCreateInfo, nullptr, &Swapchain));
    }

    VulkanSwapchain::~VulkanSwapchain() {
        vkDestroySwapchainKHR(Device, Swapchain, nullptr);
    }
}
