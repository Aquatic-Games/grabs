#pragma once

#include <memory>

#include "Surface.h"
#include "Swapchain.h"
#include "CommandList.h"

namespace grabs
{
    class Device
    {
    public:
        virtual ~Device() = default;

        virtual std::unique_ptr<Swapchain> CreateSwapchain(const SwapchainDescription& description, Surface* surface) = 0;

        virtual std::unique_ptr<CommandList> CreateCommandList() = 0;

        virtual void SubmitCommandList(CommandList* list) = 0;
    };
}
