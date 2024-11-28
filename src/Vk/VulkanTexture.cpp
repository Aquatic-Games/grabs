#include "VulkanTexture.h"

namespace grabs {
    Size3D Vk::VulkanTexture::Size() const
    {
        return {};
    }

    Format Vk::VulkanTexture::Format() const
    {
        return Format::D32_Float;
    }
}