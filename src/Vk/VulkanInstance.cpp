#include "VulkanInstance.h"

namespace grabs::Vk {
    VulkanInstance::VulkanInstance(bool debug) {
        VkApplicationInfo appInfo {
            .sType = VK_STRUCTURE_TYPE_APPLICATION_INFO,
            .pApplicationName = "GRABS",
            .applicationVersion = VK_MAKE_VERSION(1, 0, 0),
            .pEngineName = "GRABS",
            .engineVersion = VK_MAKE_VERSION(1, 0, 0),
            .apiVersion = VK_VERSION_1_3
        };

        VkInstanceCreateInfo instanceInfo {
            .sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
            .pApplicationInfo = &appInfo,
            .enabledLayerCount = ,
            .ppEnabledLayerNames = ,
            .enabledExtensionCount = ,
            .ppEnabledExtensionNames =
        };
    }
}
