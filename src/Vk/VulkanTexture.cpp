#include "VulkanTexture.h"

#include "../Common.h"

namespace grabs {
    Vk::VulkanTexture::VulkanTexture(VkDevice device, VkImageView view)
    {
        Device = device;
        View = view;
    }

    Vk::VulkanTexture::~VulkanTexture()
    {
        vkDestroyImageView(Device, View, nullptr);

        if (Image)
            vkDestroyImage(Device, Image, nullptr);
    }

    Size3D Vk::VulkanTexture::Size() const
    {
        GS_TODO
    }

    Format Vk::VulkanTexture::Format() const
    {
        GS_TODO
    }
}
