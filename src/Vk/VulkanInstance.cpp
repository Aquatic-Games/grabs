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

    std::vector<Adapter> VulkanInstance::EnumerateAdapters() {
        uint32_t numDevices;
        vkEnumeratePhysicalDevices(Instance, &numDevices, nullptr);
        std::vector<VkPhysicalDevice> devices(numDevices);
        vkEnumeratePhysicalDevices(Instance, &numDevices, devices.data());

        std::vector<Adapter> adapters;

        for (uint32_t i = 0; i < numDevices; i++) {
            auto device = devices[i];

            VkPhysicalDeviceProperties props;
            VkPhysicalDeviceFeatures features;
            VkPhysicalDeviceMemoryProperties mem;
            vkGetPhysicalDeviceProperties(device, &props);
            vkGetPhysicalDeviceFeatures(device, &features);
            vkGetPhysicalDeviceMemoryProperties(device, &mem);

            uint64_t memory = 0;
            if (mem.memoryHeapCount > 0) {
                memory = mem.memoryHeaps[0].size;
            }

            AdapterSupports supports {
                .GeometryShader = static_cast<bool>(features.geometryShader),
                .Anisotropy = static_cast<bool>(features.samplerAnisotropy),
                .MaxAnisotropy = props.limits.maxSamplerAnisotropy
            };

            Adapter adapter {
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
