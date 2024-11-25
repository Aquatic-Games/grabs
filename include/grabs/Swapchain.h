#pragma once

#include "Common.h"
#include "TextureView.h"

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

        virtual TextureView* GetNextTexture() = 0;

        virtual void Present() = 0;
    };
}
