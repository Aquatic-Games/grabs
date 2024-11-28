#include "VulkanCommandList.h"

#include "VkUtils.h"
#include "../Common.h"

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

    void VulkanCommandList::Begin()
    {
        GS_TODO
    }

    void VulkanCommandList::End()
    {
        GS_TODO
    }

    void VulkanCommandList::BeginRenderPass(const RenderPassDescription& description)
    {
        GS_TODO
    }

    void VulkanCommandList::EndRenderPass()
    {
        GS_TODO
    }

    void VulkanCommandList::SetViewport(const Viewport& viewport)
    {
        GS_TODO
    }

    void VulkanCommandList::SetPipeline(Pipeline* pipeline)
    {
        GS_TODO
    }

    void VulkanCommandList::SetVertexBuffer(uint32_t slot, Buffer* buffer, uint32_t stride, uint32_t offset)
    {
        GS_TODO
    }

    void VulkanCommandList::SetIndexBuffer(Buffer* buffer, Format format)
    {
        GS_TODO
    }

    void VulkanCommandList::DrawIndexed(uint32_t numElements)
    {
        GS_TODO
    }
}
