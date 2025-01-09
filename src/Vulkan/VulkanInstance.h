#pragma once

#include <vulkan/vulkan.h>

#include "grabs/Instance.h"

namespace grabs::Vulkan
{
    class VulkanInstance final : public Instance
    {
    public:
        VkInstance Instance;
        VkDebugUtilsMessengerEXT DebugMessenger;

        explicit VulkanInstance(const InstanceInfo& info);
        ~VulkanInstance() override;

        std::vector<Adapter> EnumerateAdapters() override;
    };
}
