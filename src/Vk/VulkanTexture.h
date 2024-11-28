#pragma once

#include <vulkan/vulkan.h>

#include "grabs/Texture.h"

namespace grabs::Vk
{
    class VulkanTexture final : public Texture
    {
    public:
        VkDevice Device{};

        VkImage Image{};
        VkImageView View{};

        // Used for swapchain textures.
        VulkanTexture(VkDevice device, VkImageView view);
        ~VulkanTexture() override;

        Size3D Size() const override;
        grabs::Format Format() const override;
    };

}
