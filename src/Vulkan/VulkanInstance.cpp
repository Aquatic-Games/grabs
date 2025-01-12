#include "VulkanInstance.h"

#include <vector>

#ifdef GS_OS_WINDOWS
#include <windows.h>
#include <vulkan/vulkan_win32.h>
#endif
#ifdef GS_OS_LINUX
#include <X11/Xlib-xcb.h>
#include <vulkan/vulkan_xlib.h>
#include <vulkan/vulkan_xcb.h>
#include <vulkan/vulkan_wayland.h>
#endif

#include "VulkanUtils.h"
#include "VulkanSurface.h"

namespace grabs::Vulkan
{
    VulkanInstance::VulkanInstance(const InstanceInfo& info)
    {
        std::vector extensions { VK_KHR_SURFACE_EXTENSION_NAME };
        std::vector<const char*> layers;

#if defined(GS_OS_WINDOWS)
        extensions.push_back(VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
#elif defined(GS_OS_LINUX)
        extensions.push_back(VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
        extensions.push_back(VK_KHR_XCB_SURFACE_EXTENSION_NAME);
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
        std::vector<Adapter> adapters;

        uint32_t numDevices;
        vkEnumeratePhysicalDevices(Instance, &numDevices, nullptr);
        std::vector<VkPhysicalDevice> physDevices(numDevices);
        vkEnumeratePhysicalDevices(Instance, &numDevices, physDevices.data());

        int i = 0;
        for (VkPhysicalDevice device : physDevices)
        {
            VkPhysicalDeviceProperties properties;
            vkGetPhysicalDeviceProperties(device, &properties);

            VkPhysicalDeviceMemoryProperties memoryProps;
            vkGetPhysicalDeviceMemoryProperties(device, &memoryProps);

            AdapterType type;
            switch (properties.deviceType)
            {
                case VK_PHYSICAL_DEVICE_TYPE_OTHER:
                    type = AdapterType::Unknown;
                    break;
                case VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU:
                    type = AdapterType::Integrated;
                    break;
                case VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU:
                    type = AdapterType::Discrete;
                    break;
                case VK_PHYSICAL_DEVICE_TYPE_VIRTUAL_GPU:
                    type = AdapterType::Unknown;
                    break;
                case VK_PHYSICAL_DEVICE_TYPE_CPU:
                    type = AdapterType::Software;
                    break;
                default:
                    type = AdapterType::Unknown;
            }

            adapters.push_back({
                .Index = i++,
                .Name = properties.deviceName,
                .Type = type,
                .DedicatedMemory = memoryProps.memoryHeaps[0].size // TODO: Make this more robust
            });
        }

        return adapters;
    }

    std::unique_ptr<Surface> VulkanInstance::CreateSurface(const SurfaceDescription& description)
    {
        return std::make_unique<VulkanSurface>(Instance, description);
    }
}
