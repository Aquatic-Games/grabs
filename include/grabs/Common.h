#pragma once

#include <cstdint>

namespace grabs
{
    enum class Backend
    {
        Unknown = 0,
        Vulkan = 1 << 0
    };

    enum class AdapterType
    {
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
