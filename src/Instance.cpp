#include "grabs/Instance.h"

#include "Vulkan/VulkanInstance.h"

namespace grabs
{
    std::unique_ptr<Instance> Instance::Create(const InstanceInfo& info)
    {
        return std::make_unique<Vulkan::VulkanInstance>(info);
    }

}