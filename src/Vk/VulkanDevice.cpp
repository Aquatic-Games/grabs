#include "VulkanDevice.h"

#include <optional>
#include <stdexcept>
#include <vector>
#include <set>

#include "VkUtils.h"
#include "VulkanSwapchain.h"

namespace grabs::Vk {
    VulkanDevice::VulkanDevice(VkInstance instance, VulkanSurface* surface, uint32_t adapterIndex) {
        Instance = instance;

        uint32_t numDevices;
        vkEnumeratePhysicalDevices(instance, &numDevices, nullptr);
        std::vector<VkPhysicalDevice> devices(numDevices);
        vkEnumeratePhysicalDevices(instance, &numDevices, devices.data());

        VkPhysicalDevice device = devices[adapterIndex];
        PhysicalDevice = device;

        uint32_t numFamilies;
        vkGetPhysicalDeviceQueueFamilyProperties(device, &numFamilies, nullptr);
        std::vector<VkQueueFamilyProperties> queueFamilies(numFamilies);
        vkGetPhysicalDeviceQueueFamilyProperties(device, &numFamilies, queueFamilies.data());

        std::optional<uint32_t> graphicsQueue;
        std::optional<uint32_t> presentQueue;

        auto surfaceKhr = surface->Surface;

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

        GraphicsQueueIndex = graphicsQueue.value();
        PresentQueueIndex = presentQueue.value();

        std::set uniqueFamilies { GraphicsQueueIndex, PresentQueueIndex };

        std::vector<VkDeviceQueueCreateInfo> queueCreateInfos;

        float queuePriority = 1.0f;
        for (const auto family : uniqueFamilies) {
            VkDeviceQueueCreateInfo createInfo {
                .sType = VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                .queueFamilyIndex = family,
                .queueCount = 1,
                .pQueuePriorities = &queuePriority
            };

            queueCreateInfos.push_back(createInfo);
        }

        VkPhysicalDeviceFeatures features;
        vkGetPhysicalDeviceFeatures(device, &features);

        std::vector<const char*> extensions;
        extensions.push_back(VK_KHR_SWAPCHAIN_EXTENSION_NAME);

        VkDeviceCreateInfo createInfo {
            .sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
            .queueCreateInfoCount = static_cast<uint32_t>(queueCreateInfos.size()),
            .pQueueCreateInfos = queueCreateInfos.data(),
            .enabledExtensionCount = static_cast<uint32_t>(extensions.size()),
            .ppEnabledExtensionNames = extensions.data(),
            .pEnabledFeatures = &features
        };

        CHECK_RESULT(vkCreateDevice(device, &createInfo, nullptr, &Device));

        vkGetDeviceQueue(Device, GraphicsQueueIndex, 0, &GraphicsQueue);
        vkGetDeviceQueue(Device, PresentQueueIndex, 0, &PresentQueue);
    }

    VulkanDevice::~VulkanDevice() {
        vkDestroyDevice(Device, nullptr);
    }

    std::unique_ptr<Swapchain> VulkanDevice::CreateSwapchain(const SwapchainDescription& description, Surface* surface) {
        return std::make_unique<VulkanSwapchain>(Instance, PhysicalDevice, this, description, dynamic_cast<VulkanSurface*>(surface));
    }
}
