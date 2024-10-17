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

    VkFormat FormatToVk(Format format) {
        switch (format) {
            case Format::B5G6R5_UNorm:
                return VK_FORMAT_B5G6R5_UNORM_PACK16;
            case Format::B5G5R5A1_UNorm:
                return VK_FORMAT_B5G5R5A1_UNORM_PACK16;
            case Format::R8_UNorm:
                return VK_FORMAT_R8_UNORM;
            case Format::R8_UInt:
                return VK_FORMAT_R8_UINT;
            case Format::R8_SNorm:
                return VK_FORMAT_R8_SNORM;
            case Format::R8_SInt:
                return VK_FORMAT_R8_SINT;
            case Format::A8_UNorm:
                return VK_FORMAT_A8_UNORM_KHR;
            case Format::R8G8_UNorm:
                return VK_FORMAT_R8G8_UNORM;
            case Format::R8G8_UInt:
                return VK_FORMAT_R8G8_UINT;
            case Format::R8G8_SNorm:
                return VK_FORMAT_R8G8_SNORM;
            case Format::R8G8_SInt:
                return VK_FORMAT_R8G8_SINT;
            case Format::R8G8B8A8_UNorm:
                return VK_FORMAT_R8G8B8A8_UNORM;
            case Format::R8G8B8A8_UNorm_SRGB:
                return VK_FORMAT_R8G8B8A8_SRGB;
            case Format::R8G8B8A8_UInt:
                return VK_FORMAT_R8G8B8A8_UINT;
            case Format::R8G8B8A8_SNorm:
                return VK_FORMAT_R8G8B8A8_SNORM;
            case Format::R8G8B8A8_SInt:
                return VK_FORMAT_R8G8B8A8_SINT;
            case Format::B8G8R8A8_UNorm:
                return VK_FORMAT_B8G8R8A8_UNORM;
            case Format::B8G8R8A8_UNorm_SRGB:
                return VK_FORMAT_B8G8R8A8_SRGB;
            //case Format::R10G10B10A2_UNorm:
            //    return VK_FORMAT_A2R10G10B10_UNORM_PACK32;
            //case Format::R10G10B10A2_UInt:
            //    return VK_FORMAT_A2R10G10B10_UINT_PACK32;
            //case Format::R11G11B10_Float:
            //    return VK_FORMAT_R11G11B10
            case Format::R16_Float:
                return VK_FORMAT_R16_SFLOAT;
            case Format::D16_UNorm:
                return VK_FORMAT_D16_UNORM;
            case Format::R16_UNorm:
                return VK_FORMAT_R16_UNORM;
            case Format::R16_UInt:
                return VK_FORMAT_R16_UINT;
            case Format::R16_SNorm:
                return VK_FORMAT_R16_SNORM;
            case Format::R16_SInt:
                return VK_FORMAT_R16_SINT;
            case Format::R16G16_Float:
                return VK_FORMAT_R16G16_SFLOAT;
            case Format::R16G16_UNorm:
                return VK_FORMAT_R16G16_UNORM;
            case Format::R16G16_UInt:
                return VK_FORMAT_R16G16_UINT;
            case Format::R16G16_SNorm:
                return VK_FORMAT_R16G16_SNORM;
            case Format::R16G16_SInt:
                return VK_FORMAT_R16G16_SINT;
            case Format::R16G16B16A16_Float:
                return VK_FORMAT_R16G16B16A16_SFLOAT;
            case Format::R16G16B16A16_UNorm:
                return VK_FORMAT_R16G16B16A16_UNORM;
            case Format::R16G16B16A16_UInt:
                return VK_FORMAT_R16G16B16A16_UINT;
            case Format::R16G16B16A16_SNorm:
                return VK_FORMAT_R16G16B16A16_SNORM;
            case Format::R16G16B16A16_SInt:
                return VK_FORMAT_R16G16B16A16_SINT;
            case Format::R32_Float:
                return VK_FORMAT_R32_SFLOAT;
            case Format::R32_UInt:
                return VK_FORMAT_R32_UINT;
            case Format::R32_SInt:
                return VK_FORMAT_R32_SINT;
            case Format::R32G32_Float:
                return VK_FORMAT_R32G32_SFLOAT;
            case Format::R32G32_UInt:
                return VK_FORMAT_R32G32_UINT;
            case Format::R32G32_SInt:
                return VK_FORMAT_R32G32_SINT;
            case Format::R32G32B32_Float:
                return VK_FORMAT_R32G32B32_SFLOAT;
            case Format::R32G32B32_UInt:
                return VK_FORMAT_R32G32B32_UINT;
            case Format::R32G32B32_SInt:
                return VK_FORMAT_R32G32B32_SINT;
            case Format::R32G32B32A32_Float:
                return VK_FORMAT_R32G32B32A32_SFLOAT;
            case Format::R32G32B32A32_UInt:
                return VK_FORMAT_R32G32B32A32_UINT;
            case Format::R32G32B32A32_SInt:
                return VK_FORMAT_R32G32B32A32_SINT;
            case Format::D24_UNorm_S8_UInt:
                return VK_FORMAT_D24_UNORM_S8_UINT;
            case Format::D32_Float:
                return VK_FORMAT_D32_SFLOAT;
            case Format::BC1_UNorm:
                return VK_FORMAT_BC1_RGBA_UNORM_BLOCK;
            case Format::BC1_UNorm_SRGB:
                return VK_FORMAT_BC1_RGBA_SRGB_BLOCK;
            case Format::BC2_UNorm:
                return VK_FORMAT_BC2_UNORM_BLOCK;
            case Format::BC2_UNorm_SRGB:
                return VK_FORMAT_BC2_UNORM_BLOCK;
            case Format::BC3_UNorm:
                return VK_FORMAT_BC3_UNORM_BLOCK;
            case Format::BC3_UNorm_SRGB:
                return VK_FORMAT_BC3_SRGB_BLOCK;
            case Format::BC4_UNorm:
                return VK_FORMAT_BC4_UNORM_BLOCK;
            case Format::BC4_SNorm:
                return VK_FORMAT_BC4_SNORM_BLOCK;
            case Format::BC5_UNorm:
                return VK_FORMAT_BC5_UNORM_BLOCK;
            case Format::BC5_SNorm:
                return VK_FORMAT_BC5_SNORM_BLOCK;
            case Format::BC6H_UF16:
                return VK_FORMAT_BC6H_UFLOAT_BLOCK;
            case Format::BC6H_SF16:
                return VK_FORMAT_BC6H_SFLOAT_BLOCK;
            case Format::BC7_UNorm:
                return VK_FORMAT_BC7_UNORM_BLOCK;
            case Format::BC7_UNorm_SRGB:
                return VK_FORMAT_BC7_SRGB_BLOCK;
        }

        return VK_FORMAT_UNDEFINED;
    }

    VkPresentModeKHR PresentModeToVk(PresentMode mode) {
        switch (mode) {
            case PresentMode::Immediate:
                return VK_PRESENT_MODE_IMMEDIATE_KHR;
            case PresentMode::Mailbox:
                return VK_PRESENT_MODE_MAILBOX_KHR;
            case PresentMode::Fifo:
                return VK_PRESENT_MODE_FIFO_KHR;
        }

        return VK_PRESENT_MODE_MAX_ENUM_KHR;
    }
}
