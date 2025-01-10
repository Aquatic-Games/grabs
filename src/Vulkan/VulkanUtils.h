#pragma once

#include <format>
#include <string>
#include <stdexcept>

#include <vulkan/vulkan.h>

#define VK_CHECK_RESULT(Result) {\
    VkResult res = Result;\
    if (res != VK_SUCCESS)\
        throw std::runtime_error(std::format("{}:{}: {} failed with error {}", __FILE__, __LINE__, #Result, grabs::Vulkan::ResultToString(res)));\
}

namespace grabs::Vulkan
{
    inline std::string ResultToString(VkResult result)
    {
    #define STR(Result) case VK_##Result: return #Result;

        switch (result)
        {
            STR(SUCCESS)
            STR(NOT_READY)
            STR(TIMEOUT)
            STR(EVENT_SET)
            STR(EVENT_RESET)
            STR(INCOMPLETE)
            STR(ERROR_OUT_OF_HOST_MEMORY)
            STR(ERROR_OUT_OF_DEVICE_MEMORY)
            STR(ERROR_INITIALIZATION_FAILED)
            STR(ERROR_DEVICE_LOST)
            STR(ERROR_MEMORY_MAP_FAILED)
            STR(ERROR_LAYER_NOT_PRESENT)
            STR(ERROR_EXTENSION_NOT_PRESENT)
            STR(ERROR_FEATURE_NOT_PRESENT)
            STR(ERROR_INCOMPATIBLE_DRIVER)
            STR(ERROR_TOO_MANY_OBJECTS)
            STR(ERROR_FORMAT_NOT_SUPPORTED)
            STR(ERROR_FRAGMENTED_POOL)
            STR(ERROR_UNKNOWN)
            STR(ERROR_OUT_OF_POOL_MEMORY)
            STR(ERROR_INVALID_EXTERNAL_HANDLE)
            STR(ERROR_FRAGMENTATION)
            STR(ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS)
            STR(PIPELINE_COMPILE_REQUIRED)
            STR(ERROR_SURFACE_LOST_KHR)
            STR(ERROR_NATIVE_WINDOW_IN_USE_KHR)
            STR(SUBOPTIMAL_KHR)
            STR(ERROR_OUT_OF_DATE_KHR)
            STR(ERROR_INCOMPATIBLE_DISPLAY_KHR)
            STR(ERROR_VALIDATION_FAILED_EXT)
            STR(ERROR_INVALID_SHADER_NV)
            STR(ERROR_IMAGE_USAGE_NOT_SUPPORTED_KHR)
            STR(ERROR_VIDEO_PICTURE_LAYOUT_NOT_SUPPORTED_KHR)
            STR(ERROR_VIDEO_PROFILE_OPERATION_NOT_SUPPORTED_KHR)
            STR(ERROR_VIDEO_PROFILE_FORMAT_NOT_SUPPORTED_KHR)
            STR(ERROR_VIDEO_PROFILE_CODEC_NOT_SUPPORTED_KHR)
            STR(ERROR_VIDEO_STD_VERSION_NOT_SUPPORTED_KHR)
            STR(ERROR_INVALID_DRM_FORMAT_MODIFIER_PLANE_LAYOUT_EXT)
            STR(ERROR_NOT_PERMITTED_KHR)
            STR(ERROR_FULL_SCREEN_EXCLUSIVE_MODE_LOST_EXT)
            STR(THREAD_IDLE_KHR)
            STR(THREAD_DONE_KHR)
            STR(OPERATION_DEFERRED_KHR)
            STR(OPERATION_NOT_DEFERRED_KHR)
            STR(ERROR_INVALID_VIDEO_STD_PARAMETERS_KHR)
            STR(ERROR_COMPRESSION_EXHAUSTED_EXT)
            STR(INCOMPATIBLE_SHADER_BINARY_EXT)
            STR(PIPELINE_BINARY_MISSING_KHR)
            STR(ERROR_NOT_ENOUGH_SPACE_KHR)

            default:
                return "UNKNOWN";
        }

    #undef STR
    }
}