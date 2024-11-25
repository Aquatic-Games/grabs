#pragma once

#include <cstdint>

namespace grabs
{
    enum class BufferType
    {
        Vertex,
        Index,
        Constant
    };

    struct BufferDescription
    {
        BufferType Type;
        uint32_t Size;
        bool Dynamic;
    };

    class Buffer
    {
    public:
        virtual ~Buffer() = default;
    };
}
