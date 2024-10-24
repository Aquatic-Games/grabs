#pragma once

#include <vulkan/vulkan_core.h>

#include "grabs/CommandList.h"

namespace grabs::Vk {

    class VulkanCommandList : public CommandList {
    public:
        VkDevice Device;
        VkCommandPool Pool;
        VkCommandBuffer CommandBuffer;

        explicit VulkanCommandList(VkDevice device, VkCommandPool pool);
        ~VulkanCommandList() override;
    };

}
