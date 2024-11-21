#pragma once

#include <string>
#include <cstdint>

#define GS_FLAGS_ENUM(Enum)\
    inline Enum operator |(Enum left, Enum right) { return static_cast<Enum>(static_cast<int>(left) | static_cast<int>(right)); }\
    inline int operator &(Enum left, Enum right) { return static_cast<int>(left) & static_cast<int>(right); }

namespace grabs
{
    struct Size2D
    {
        uint32_t Width;
        uint32_t Height;
    };

    struct Size3D
    {
        uint32_t Width;
        uint32_t Height;
        uint32_t Depth;
    };

    enum class Backend
    {
        Unknown = 0,
        Vulkan = 1 << 0,
        D3D11 = 1 << 1
    };
    GS_FLAGS_ENUM(Backend)

    inline std::string BackendToFriendlyString(const Backend backend)
    {
        switch (backend)
        {
            case Backend::Vulkan:
                return "Vulkan";
            case Backend::D3D11:
                return "DirectX 11";
            case Backend::Unknown:
                return "None";
        }

        return "Unknown";
    }

    enum class Format
    {
        B5G6R5_UNorm,
        B5G5R5A1_UNorm,

        R8_UNorm,
        R8_UInt,
        R8_SNorm,
        R8_SInt,
        A8_UNorm,

        R8G8_UNorm,
        R8G8_UInt,
        R8G8_SNorm,
        R8G8_SInt,

        R8G8B8A8_UNorm,
        R8G8B8A8_UNorm_SRGB,
        R8G8B8A8_UInt,
        R8G8B8A8_SNorm,
        R8G8B8A8_SInt,

        B8G8R8A8_UNorm,
        B8G8R8A8_UNorm_SRGB,

        //R10G10B10A2_UNorm,
        //R10G10B10A2_UInt,
        //R11G11B10_Float,

        R16_Float,
        D16_UNorm,
        R16_UNorm,
        R16_UInt,
        R16_SNorm,
        R16_SInt,

        R16G16_Float,
        R16G16_UNorm,
        R16G16_UInt,
        R16G16_SNorm,
        R16G16_SInt,

        R16G16B16A16_Float,
        R16G16B16A16_UNorm,
        R16G16B16A16_UInt,
        R16G16B16A16_SNorm,
        R16G16B16A16_SInt,

        R32_Float,
        R32_UInt,
        R32_SInt,

        R32G32_Float,
        R32G32_UInt,
        R32G32_SInt,

        R32G32B32_Float,
        R32G32B32_UInt,
        R32G32B32_SInt,

        R32G32B32A32_Float,
        R32G32B32A32_UInt,
        R32G32B32A32_SInt,

        D24_UNorm_S8_UInt,
        D32_Float,

        BC1_UNorm,
        BC1_UNorm_SRGB,

        BC2_UNorm,
        BC2_UNorm_SRGB,

        BC3_UNorm,
        BC3_UNorm_SRGB,

        BC4_UNorm,
        BC4_SNorm,

        BC5_UNorm,
        BC5_SNorm,

        BC6H_UF16,
        BC6H_SF16,

        BC7_UNorm,
        BC7_UNorm_SRGB
    };

    struct AdapterSupports
    {
        bool GeometryShader;

        bool Anisotropy;
        float MaxAnisotropy;
    };

    struct Adapter
    {
        uint32_t Index;
        std::string Name;
        uint64_t Memory;
        AdapterSupports Supports;
    };
}
