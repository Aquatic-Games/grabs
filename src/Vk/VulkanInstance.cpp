#include "VulkanInstance.h"
#include "../Common.h"

#include <vector>

#include "VkUtils.h"

namespace grabs::Vk {
    VulkanInstance::VulkanInstance(const InstanceInfo& info) {
        NULL_CHECK(info.CreateSurface);
        NULL_CHECK(info.GetInstanceExtensions);

        VkApplicationInfo appInfo {
            .sType = VK_STRUCTURE_TYPE_APPLICATION_INFO,
            .pApplicationName = "GRABS",
            .applicationVersion = VK_MAKE_VERSION(1, 0, 0),
            .pEngineName = "GRABS",
            .engineVersion = VK_MAKE_VERSION(1, 0, 0),
            .apiVersion = VK_API_VERSION_1_3
        };

        auto extensions = info.GetInstanceExtensions();

        std::vector<const char*> layers;
        if (info.Debug) {
            layers.push_back("VK_LAYER_KHRONOS_validation");
        }

        VkInstanceCreateInfo instanceInfo {
            .sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
            .pApplicationInfo = &appInfo,
            .enabledLayerCount = static_cast<uint32_t>(layers.size()),
            .ppEnabledLayerNames = layers.data(),
            .enabledExtensionCount = static_cast<uint32_t>(extensions.size()),
            .ppEnabledExtensionNames = extensions.data()
        };

        CHECK_RESULT(vkCreateInstance(&instanceInfo, nullptr, &Instance));
    }

    VulkanInstance::~VulkanInstance() {
        vkDestroyInstance(Instance, nullptr);
    }
}
