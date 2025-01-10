#pragma once

#include <vulkan/vulkan.h>

#include "grabs/Instance.h"

namespace grabs::Vulkan
{
    class VulkanInstance final : public Instance
    {
    public:
        VkInstance Instance;
        VkDebugUtilsMessengerEXT DebugMessenger;

        explicit VulkanInstance(const InstanceInfo& info);
        ~VulkanInstance() override;

        std::vector<Adapter> EnumerateAdapters() override;

        // TODO: Better way of doing this?
        // I'm not overly a fan of this solution, I'd much prefer Surface::Xlib(...) but this is the only way I can
        // think of doing this right now.
        std::unique_ptr<Surface> CreateSurface(const SurfaceDescription& description) override;
    };
}
