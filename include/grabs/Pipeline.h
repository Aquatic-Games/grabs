#pragma once

#include <cstdint>

#include "grabs/Common.h"
#include "ShaderModule.h"

namespace grabs
{
    enum class InputType
    {
        PerVertex,
        PerInstance
    };

    struct InputLayoutDescription
    {
        Format Format;
        uint32_t Offset;
        uint32_t Slot;
        InputType Type;
    };

    struct PipelineDescription
    {
        ShaderModule* VertexShader;
        ShaderModule* PixelShader;
        std::vector<InputLayoutDescription> InputLayout;
    };

    class Pipeline
    {
    public:
        virtual ~Pipeline() = default;
    };
}
