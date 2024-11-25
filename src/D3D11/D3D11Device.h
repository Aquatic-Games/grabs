#pragma once

#include "grabs/Device.h"

#include <d3d11.h>

namespace grabs::D3D11
{
    class D3D11Device : public Device
    {
    public:
        IDXGIFactory1* Factory{};
        ID3D11Device* Device{};
        ID3D11DeviceContext* Context{};

        D3D11Device(IDXGIFactory1* factory, IDXGIAdapter1* adapter, bool debug);
        ~D3D11Device() override;

        std::unique_ptr<Swapchain> CreateSwapchain(const SwapchainDescription& description, Surface* surface) override;
        std::unique_ptr<CommandList> CreateCommandList() override;
    };
}