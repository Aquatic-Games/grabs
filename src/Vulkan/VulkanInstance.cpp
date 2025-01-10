#include "VulkanInstance.h"

#include <vector>

#if defined(_WIN32)
#include <vulkan/vulkan_win32.h>
#elif defined(__unix__)
//#include <vulkan/vulkan_xlib.h>
//#include <vulkan/vulkan_xcb.h>
#include <vulkan/vulkan_wayland.h>
#endif

#include "VulkanUtils.h"

namespace grabs::Vulkan
{
    VulkanInstance::VulkanInstance(const InstanceInfo& info)
    {
        std::vector extensions { VK_KHR_SURFACE_EXTENSION_NAME };
        std::vector<const char*> layers;

#if defined(_WIN32)
        extensions.push_back(VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
#elif defined(__unix__)
        //extensions.push_back(VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
        //extensions.push_back(VK_KHR_XCB_SURFACE_EXTENSION_NAME);
        extensions.push_back(VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME);
#else
#error "Unsupported OS"
#endif

        if (info.Debug)
        {
            extensions.push_back(VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
            layers.push_back("VK_LAYER_KHRONOS_validation");
        }

        VkApplicationInfo appInfo
        {
            .sType = VK_STRUCTURE_TYPE_APPLICATION_INFO,
            .pApplicationName = info.AppName.c_str(),
            .applicationVersion = 0,
            .pEngineName = "GRABS",
            .engineVersion = 0,
            .apiVersion = VK_API_VERSION_1_3
        };

        VkInstanceCreateInfo instanceInfo
        {
            .sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
            .pApplicationInfo = &appInfo,
            .enabledLayerCount = static_cast<uint32_t>(layers.size()),
            .ppEnabledLayerNames = layers.data(),
            .enabledExtensionCount = static_cast<uint32_t>(extensions.size()),
            .ppEnabledExtensionNames = extensions.data()
        };

        VK_CHECK_RESULT(vkCreateInstance(&instanceInfo, nullptr, &Instance));
    }

    VulkanInstance::~VulkanInstance()
    {
        vkDestroyInstance(Instance, nullptr);
    }

    std::vector<Adapter> VulkanInstance::EnumerateAdapters()
    {
        return {};
    }
}
