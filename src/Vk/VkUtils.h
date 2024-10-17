#pragma once

#include <vulkan/vulkan.h>

#include <string>
#include <stdexcept>

#define CHECK_RESULT(result) \
{ \
    VkResult res = (result); \
    if (res != VK_SUCCESS) { \
        throw std::runtime_error(std::string(__FILE__) + ":" + std::to_string(__LINE__) + ": " + std::string(#result) + " failed with error: " + grabs::Vk::Utils::ResultToString(res)); \
    } \
}

namespace grabs::Vk::Utils {
    std::string ResultToString(VkResult result);

}