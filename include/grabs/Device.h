#pragma once

#include <memory>
#include <span>

#include "Surface.h"
#include "Swapchain.h"
#include "CommandList.h"
#include "Buffer.h"

namespace grabs
{
    class Device
    {
    public:
        virtual ~Device() = default;

        virtual std::unique_ptr<Swapchain> CreateSwapchain(const SwapchainDescription& description, Surface* surface) = 0;
        virtual std::unique_ptr<CommandList> CreateCommandList() = 0;
        virtual std::unique_ptr<Buffer> CreateBuffer(const BufferDescription& description, void* data) = 0;

        virtual void SubmitCommandList(CommandList* list) = 0;
    };
}
