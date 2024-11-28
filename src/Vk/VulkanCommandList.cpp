#include "VulkanCommandList.h"

#include "VkUtils.h"

namespace grabs::Vk
{
    VulkanCommandList::VulkanCommandList(VkDevice device, VkCommandPool pool)
    {
        Device = device;
        Pool = pool;

        VkCommandBufferAllocateInfo cbInfo
        {
            .sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            .commandPool = pool,
            .level = VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            .commandBufferCount = 1
        };

        VK_CHECK_RESULT(vkAllocateCommandBuffers(device, &cbInfo, &CommandBuffer));
    }

    VulkanCommandList::~VulkanCommandList()
    {
        vkFreeCommandBuffers(Device, Pool, 1, &CommandBuffer);
    }
}
