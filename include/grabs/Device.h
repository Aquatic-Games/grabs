#pragma once

#include <memory>

#include "Surface.h"
#include "Swapchain.h"

namespace grabs {

    class Device {
    public:
        virtual ~Device() = default;

        virtual std::unique_ptr<Swapchain> CreateSwapchain(const SwapchainDescription& description, Surface* surface) = 0;
    };

}
