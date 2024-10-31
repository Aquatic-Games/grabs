#include "VulkanTexture.h"

namespace grabs {
    Size3D Vk::VulkanTexture::Size() const {
        return {};
    }

    grabs::Format Vk::VulkanTexture::Format() const {
        return Format::D32_Float;
    }
} // grabs