#pragma once

#include <vulkan/vulkan.h>

#include "grabs/Device.h"
#include "grabs/Surface.h"

namespace grabs::Vk {

    class VulkanDevice : public Device {
    public:
        VkDevice Device;

        uint32_t GraphicsQueueIndex;
        uint32_t PresentQueueIndex;

        VulkanDevice(VkInstance instance, VulkanSurface* surface, uint32_t adapterIndex);
        ~VulkanDevice() override;
    };

}
