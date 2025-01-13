#include "VulkanInstance.h"

#include <vector>
#include <format>
#include <iostream>

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

struct CallbackData
{
    grabs::GrabsDebugCallback Callback;
    void* Data;
};

VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity,
                       VkDebugUtilsMessageTypeFlagsEXT messageTypes,
                       const VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
{
    auto message = pCallbackData->pMessage;

    if (GS_HAS_FLAG(messageSeverity, VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT))
        throw std::runtime_error(std::format("Debug error: {}", message));

    auto callbackData = *static_cast<CallbackData*>(pUserData);

    if (callbackData.Callback)
    {
        grabs::DebugSeverity severity;
        switch (messageSeverity)
        {
            case VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT:
                severity = grabs::DebugSeverity::Verbose;
                break;
            case VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT:
                severity = grabs::DebugSeverity::Info;
                break;
            case VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT:
                severity = grabs::DebugSeverity::Warning;
                break;
            case VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT:
                severity = grabs::DebugSeverity::Error;
                break;
            default:
                severity = grabs::DebugSeverity::Verbose;
                break;
        }

        grabs::DebugType type;
        switch (messageTypes)
        {
            case VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT:
                type = grabs::DebugType::General;
                break;
            case VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT:
                type = grabs::DebugType::Validation;
                break;
            case VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT:
                type = grabs::DebugType::General;
                break;
            case VK_DEBUG_UTILS_MESSAGE_TYPE_DEVICE_ADDRESS_BINDING_BIT_EXT:
                type = grabs::DebugType::General;
                break;
            default:
                type = grabs::DebugType::General;
                break;
        }

        callbackData.Callback(severity, type, message, callbackData.Data);
    }

    return VK_TRUE;
}

namespace grabs::Vulkan
{
    VulkanInstance::VulkanInstance(const InstanceInfo& info)
    {
        std::vector extensions { VK_KHR_SURFACE_EXTENSION_NAME };
        std::vector<const char*> layers;

#ifdef GS_OS_WINDOWS
        extensions.push_back(VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
#endif
#ifdef GS_OS_LINUX
        extensions.push_back(VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
        extensions.push_back(VK_KHR_XCB_SURFACE_EXTENSION_NAME);
        extensions.push_back(VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME);
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

        if (info.Debug)
        {
            auto createDebugMessenger = reinterpret_cast<PFN_vkCreateDebugUtilsMessengerEXT>(vkGetInstanceProcAddr(
                Instance, "vkCreateDebugUtilsMessengerEXT"));

            if (!createDebugMessenger)
                throw std::runtime_error("Failed to get debug messenger proc address - Is the SDK installed?");

            CallbackData callbackData
            {
                .Callback = info.DebugCallback,
                .Data = info.DebugCallbackData
            };

            VkDebugUtilsMessengerCreateInfoEXT messengerInfo
            {
                .sType = VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
                .messageSeverity = VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT |
                                   VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT |
                                   VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT |
                                   VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT,
                .messageType = VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT |
                               VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT |
                               VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT |
                               VK_DEBUG_UTILS_MESSAGE_TYPE_DEVICE_ADDRESS_BINDING_BIT_EXT,
                .pfnUserCallback = DebugCallback,
                .pUserData = static_cast<void*>(&callbackData)
            };

            VK_CHECK_RESULT(createDebugMessenger(Instance, &messengerInfo, nullptr, &DebugMessenger));
        }
    }

    VulkanInstance::~VulkanInstance()
    {
        if (DebugMessenger)
        {
            auto destroyDebugMessenger = reinterpret_cast<PFN_vkDestroyDebugUtilsMessengerEXT>(vkGetInstanceProcAddr(
                Instance, "vkDestroyDebugUtilsMessengerEXT"));

            destroyDebugMessenger(Instance, DebugMessenger, nullptr);
        }

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
