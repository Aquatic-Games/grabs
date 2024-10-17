#pragma once

#include "Common.h"

namespace grabs {

    enum class PresentMode {
        Immediate,
        Mailbox,
        Fifo
    };

    struct SwapchainDescription {
        Size Size;
        Format Format;
        uint32_t NumBuffers;
        PresentMode PresentMode;
    };

    class Swapchain {
    public:
        virtual ~Swapchain() = default;
    };

}
