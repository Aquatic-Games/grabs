#pragma once

#include <cstdint>

#define GS_HAS_FLAG(Enum, Value) (Enum & Value) == Value

namespace grabs
{
    enum class Backend
    {
        Unknown = 0,
        Vulkan = 1 << 0
    };

    enum class AdapterType
    {
        Unknown,
        Software,
        Integrated,
        Discrete
    };

    struct Adapter
    {
        int Index;
        std::string Name;
        AdapterType Type;
        uint64_t DedicatedMemory;
    };
}
