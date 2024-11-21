#include "VulkanInstance.h"

#include <vector>
#include <iostream>
#include <stdexcept>

#include "../Common.h"
#include "VkUtils.h"
#include "VulkanDevice.h"

VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity,
                       VkDebugUtilsMessageTypeFlagsEXT messageTypes,
                       const VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
{
    if ((messageSeverity & VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT)
        throw std::runtime_error("Vulkan error: " + std::string(pCallbackData->pMessage));

    std::cout << pCallbackData->pMessage << std::endl;

    return VK_TRUE;
}

namespace grabs::Vk
{
    VulkanInstance::VulkanInstance(const InstanceInfo& info)
    {
        NULL_CHECK(info.GetInstanceExtensions);

        VkApplicationInfo appInfo
        {
            .sType = VK_STRUCTURE_TYPE_APPLICATION_INFO,
            .pApplicationName = "GRABS",
            .applicationVersion = VK_MAKE_VERSION(1, 0, 0),
            .pEngineName = "GRABS",
            .engineVersion = VK_MAKE_VERSION(1, 0, 0),
            .apiVersion = VK_API_VERSION_1_3
        };

        auto extensions = info.GetInstanceExtensions();

        std::vector<const char*> layers;
        if (info.Debug)
        {
            extensions.push_back(VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
            layers.push_back("VK_LAYER_KHRONOS_validation");
        }

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
            VkDebugUtilsMessengerCreateInfoEXT createInfo
            {
                .sType = VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
                .messageSeverity = VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT,
                .messageType = VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT | VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT,
                .pfnUserCallback = DebugCallback,
            };

            const auto createDebugMessenger = reinterpret_cast<PFN_vkCreateDebugUtilsMessengerEXT>(vkGetInstanceProcAddr(Instance, "vkCreateDebugUtilsMessengerEXT"));
            VK_CHECK_RESULT(createDebugMessenger(Instance, &createInfo, nullptr, &DebugMessenger));
        }
    }

    VulkanInstance::~VulkanInstance()
    {
        if (DebugMessenger)
        {
            const auto destroyDebugMessenger = reinterpret_cast<PFN_vkDestroyDebugUtilsMessengerEXT>(vkGetInstanceProcAddr(Instance, "vkDestroyDebugUtilsMessengerEXT"));
            destroyDebugMessenger(Instance, DebugMessenger, nullptr);
        }

        vkDestroyInstance(Instance, nullptr);
    }

    Backend VulkanInstance::Backend() const
    {
        return Backend::Vulkan;
    }

    std::unique_ptr<Device> VulkanInstance::CreateDevice(Surface* surface, uint32_t adapterIndex)
    {
        NULL_CHECK(surface);

        auto vkSurface = dynamic_cast<VulkanSurface*>(surface);

        return std::make_unique<VulkanDevice>(Instance, vkSurface, adapterIndex);
    }

    std::vector<Adapter> VulkanInstance::EnumerateAdapters()
    {
        uint32_t numDevices;
        vkEnumeratePhysicalDevices(Instance, &numDevices, nullptr);
        std::vector<VkPhysicalDevice> devices(numDevices);
        vkEnumeratePhysicalDevices(Instance, &numDevices, devices.data());

        std::vector<Adapter> adapters;

        for (uint32_t i = 0; i < numDevices; i++)
        {
            auto device = devices[i];

            VkPhysicalDeviceProperties props;
            VkPhysicalDeviceFeatures features;
            VkPhysicalDeviceMemoryProperties mem;
            vkGetPhysicalDeviceProperties(device, &props);
            vkGetPhysicalDeviceFeatures(device, &features);
            vkGetPhysicalDeviceMemoryProperties(device, &mem);

            uint64_t memory = 0;
            if (mem.memoryHeapCount > 0)
                memory = mem.memoryHeaps[0].size;

            AdapterSupports supports
            {
                .GeometryShader = static_cast<bool>(features.geometryShader),
                .Anisotropy = static_cast<bool>(features.samplerAnisotropy),
                .MaxAnisotropy = props.limits.maxSamplerAnisotropy
            };

            Adapter adapter
            {
                .Index = i,
                .Name = std::string(props.deviceName),
                .Memory = memory,
                .Supports = supports
            };

            adapters.push_back(adapter);
        }

        return adapters;
    }
}
