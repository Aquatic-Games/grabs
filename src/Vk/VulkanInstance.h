#pragma once

#include "grabs/Instance.h"

#include <vulkan/vulkan.h>

namespace grabs::Vk {

    class VulkanInstance : public Instance {
    public:
        VkInstance Instance;

        explicit VulkanInstance(const InstanceInfo& info);
        ~VulkanInstance() override;

        std::vector<Adapter> EnumerateAdapters() override;
    };

}
