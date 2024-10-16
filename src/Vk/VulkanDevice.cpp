#include "VulkanDevice.h"

#include <optional>
#include <stdexcept>
#include <vector>

namespace grabs::Vk {
    VulkanDevice::VulkanDevice(VkInstance instance, VulkanSurface* surface, uint32_t adapterIndex) {
        uint32_t numDevices;
        vkEnumeratePhysicalDevices(instance, &numDevices, nullptr);
        std::vector<VkPhysicalDevice> devices(numDevices);
        vkEnumeratePhysicalDevices(instance, &numDevices, devices.data());

        VkPhysicalDevice device = devices[adapterIndex];

        uint32_t numFamilies;
        vkGetPhysicalDeviceQueueFamilyProperties(device, &numFamilies, nullptr);
        std::vector<VkQueueFamilyProperties> queueFamilies(numFamilies);
        vkGetPhysicalDeviceQueueFamilyProperties(device, &numFamilies, queueFamilies.data());

        std::optional<uint32_t> graphicsQueue;
        std::optional<uint32_t> presentQueue;

        auto surfaceKhr = static_cast<VkSurfaceKHR>(surface->VkSurface);

        for (auto i = 0; i < numFamilies; i++) {
            if (queueFamilies[i].queueFlags & VK_QUEUE_GRAPHICS_BIT) {
                graphicsQueue = i;
            }

            VkBool32 supported;
            vkGetPhysicalDeviceSurfaceSupportKHR(device, i, surfaceKhr, &supported);

            if (supported) {
                presentQueue = i;
            }

            if (graphicsQueue.has_value() && presentQueue.has_value()) {
                break;
            }
        }

        if (!graphicsQueue.has_value() || !presentQueue.has_value()) {
            std::string unavailableQueue;
            if (!graphicsQueue.has_value()) {
                unavailableQueue = "Graphics";
            }
            if (!presentQueue.has_value()) {
                if (!unavailableQueue.empty()) {
                    unavailableQueue += " and ";
                }

                unavailableQueue += "Present";
            }

            throw std::runtime_error("Vulkan: " + unavailableQueue + " queue not available on the current adapter." );
        }

        VkDeviceCreateInfo createInfo {
            .sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
            .queueCreateInfoCount = ,
            .pQueueCreateInfos = ,
            .enabledLayerCount = ,
            .ppEnabledLayerNames = ,
            .enabledExtensionCount = ,
            .ppEnabledExtensionNames = ,
            .pEnabledFeatures =
        };
    }
}
