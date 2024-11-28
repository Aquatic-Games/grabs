#pragma once

#include "grabs/Instance.h"

#include <vulkan/vulkan.h>

namespace grabs::Vk
{
    class VulkanInstance : public Instance
    {
    public:
        VkInstance Instance{};
        VkDebugUtilsMessengerEXT DebugMessenger{};

        explicit VulkanInstance(const InstanceInfo& info);
        ~VulkanInstance() override;

        [[nodiscard]] grabs::Backend Backend() const override;

        std::unique_ptr<Device> CreateDevice(Surface* surface, const Adapter& adapter) override;

        std::vector<Adapter> EnumerateAdapters() override;
    };
}
