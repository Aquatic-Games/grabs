#pragma once

#include <d3d11.h>

#include "grabs/Swapchain.h"
#include "DXGISurface.h"
#include "D3D11Texture.h"

namespace grabs::D3D11
{
    class D3D11Swapchain : public Swapchain
    {
    public:
        IDXGISwapChain* Swapchain{};
        std::unique_ptr<D3D11Texture> SwapchainTexture{};

        D3D11Swapchain(IDXGIFactory1* factory, ID3D11Device* device, DXGISurface* surface, const SwapchainDescription& description);
        ~D3D11Swapchain() override;

        Texture* GetNextTexture() override;

        void Present() override;
    };
}
