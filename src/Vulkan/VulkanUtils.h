#pragma once

#include <format>
#include <stdexcept>

#include <vulkan/vulkan.h>

#define VK_CHECK_RESULT(Result) {\
    VkResult res = Result;\
    if (res != VK_SUCCESS)\
