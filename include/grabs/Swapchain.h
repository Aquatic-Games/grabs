#pragma once

#include "Common.h"
#include "Texture.h"

namespace grabs
{
    enum class PresentMode
    {
        Immediate,
        Mailbox,
        Fifo
    };

    struct SwapchainDescription
    {
        Size2D Size;
        Format Format;
        uint32_t NumBuffers;
        PresentMode PresentMode;
    };

    class Swapchain
    {
    public:
        virtual ~Swapchain() = default;

        virtual Texture* GetNextTexture() = 0;

        virtual void Present() = 0;
    };
}
