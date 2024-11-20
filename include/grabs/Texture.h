#pragma once

#include "Common.h"

namespace grabs
{
    class Texture
    {
    public:
        virtual ~Texture() = default;

        [[nodiscard]] virtual Size3D Size() const = 0;
        [[nodiscard]] virtual Format Format() const = 0;
    };
}
