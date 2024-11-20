#include "grabs/Instance.h"

#include <stdexcept>

#ifdef GS_ENABLE_VK
#include "Vk/VulkanInstance.h"
#endif

#ifdef GS_ENABLE_D3D11
#include "D3D11/D3D11Instance.h"
#endif

namespace grabs {
    std::unique_ptr<Instance> Instance::Create(const InstanceInfo& info) {
#ifdef GS_ENABLE_D3D11
        return std::make_unique<D3D11::D3D11Instance>(info);
#endif

#ifdef GS_ENABLE_VK
        return std::make_unique<Vk::VulkanInstance>(info);
#endif

        throw std::runtime_error("No backend available!");
    }
}