#pragma once

#include <vulkan/vulkan_core.h>

#include "grabs/CommandList.h"

namespace grabs::Vk
{
    class VulkanCommandList final : public CommandList
    {
    public:
        VkDevice Device{};
        VkCommandPool Pool{};
        VkCommandBuffer CommandBuffer{};

        explicit VulkanCommandList(VkDevice device, VkCommandPool pool);
        ~VulkanCommandList() override;

        void Begin() override;
        void End() override;

        void BeginRenderPass(const RenderPassDescription& description) override;
        void EndRenderPass() override;

        void SetViewport(const Viewport& viewport) override;
        void SetPipeline(Pipeline* pipeline) override;
        void SetVertexBuffer(uint32_t slot, Buffer* buffer, uint32_t stride, uint32_t offset) override;
        void SetIndexBuffer(Buffer* buffer, Format format) override;

        void DrawIndexed(uint32_t numElements) override;
    };
}
