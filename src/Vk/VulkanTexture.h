#pragma once

#include "grabs/Texture.h"

namespace grabs::Vk
{
    class VulkanTexture final : public Texture
    {
    public:
        ~VulkanTexture() override;

        Size3D Size() const override;
        grabs::Format Format() const override;
    };

}
