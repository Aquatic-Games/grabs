#pragma once

#include <string>
#include <vector>

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
        std::vector<uint8_t> Spirv;
        std::string EntryPoint;
    };

    class ShaderModule
    {
    public:
        virtual ~ShaderModule() = default;
    };
}
