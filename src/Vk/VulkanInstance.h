﻿#pragma once

#include "grabs/Instance.h"

#include <vulkan/vulkan.h>

namespace grabs::Vk {

    class VulkanInstance : public Instance {
    public:
        VkInstance Instance;
        VkDebugUtilsMessengerEXT DebugMessenger;

        explicit VulkanInstance(const InstanceInfo& info);
        ~VulkanInstance() override;

        std::vector<Adapter> EnumerateAdapters() override;
    };

}
