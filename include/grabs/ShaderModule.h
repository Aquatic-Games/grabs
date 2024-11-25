#pragma once

#include <string>

namespace grabs
{
    enum class ShaderStage
    {
        Vertex,
        Pixel,
        Geometry,
        Compute
    };

    struct ShaderModuleDescription
    {
        ShaderStage Stage;
        uint8_t* Spirv;
        std::string EntryPoint;
    };

    class ShaderModule
    {
    public:
        virtual ~ShaderModule() = default;
    };
}
