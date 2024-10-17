#pragma once

#include "Common.h"

namespace grabs {

    struct SwapchainDescription {
        Size Size;
        Format Format;
    };

    class Swapchain {
    public:
        virtual ~Swapchain() = default;
    };

}
