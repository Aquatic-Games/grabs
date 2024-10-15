#include "grabs/Instance.h"

#include "Vk/VulkanInstance.h"

namespace grabs {
    std::unique_ptr<Instance> Instance::Create(const InstanceInfo& info) {
        return std::make_unique<Vk::VulkanInstance>();
    }
}