#include "VkUtils.h"

#define VK_STRCASE(vkValue) case VK_##vkValue: return #vkValue;

namespace grabs::Vk::Utils {

    std::string ResultToString(VkResult result) {
        switch (result) {
            VK_STRCASE(SUCCESS)
            VK_STRCASE(NOT_READY)
            VK_STRCASE(TIMEOUT)
            VK_STRCASE(EVENT_SET)
            VK_STRCASE(EVENT_RESET)
            VK_STRCASE(INCOMPLETE)
            VK_STRCASE(ERROR_OUT_OF_HOST_MEMORY)
            VK_STRCASE(ERROR_OUT_OF_DEVICE_MEMORY)
            VK_STRCASE(ERROR_INITIALIZATION_FAILED)
            VK_STRCASE(ERROR_DEVICE_LOST)
            VK_STRCASE(ERROR_MEMORY_MAP_FAILED)
            VK_STRCASE(ERROR_LAYER_NOT_PRESENT)
            VK_STRCASE(ERROR_EXTENSION_NOT_PRESENT)
            VK_STRCASE(ERROR_FEATURE_NOT_PRESENT)
            VK_STRCASE(ERROR_INCOMPATIBLE_DRIVER)
            VK_STRCASE(ERROR_TOO_MANY_OBJECTS)
            VK_STRCASE(ERROR_FORMAT_NOT_SUPPORTED)
            VK_STRCASE(ERROR_FRAGMENTED_POOL)
            VK_STRCASE(ERROR_UNKNOWN)

            default:
                return "<UNKNOWN>";
        }
    }

}