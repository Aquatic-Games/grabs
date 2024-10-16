#pragma once

#include <string>
#include <cstdint>

namespace grabs {

    struct AdapterSupports {
        bool GeometryShader;

        bool Anisotropy;
        float MaxAnisotropy;
    };

    struct Adapter {
        uint32_t Index;
        std::string Name;
        uint64_t Memory;
        AdapterSupports Supports;
    };

}
