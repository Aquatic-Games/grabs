#pragma once

#include "grabs/Instance.h"

#include <vulkan/vulkan.h>

namespace grabs::Vk {

    class VulkanInstance : public Instance {
    public:
        VkInstance Instance;

        VulkanInstance(bool debug);
        ~VulkanInstance();
    };

}
