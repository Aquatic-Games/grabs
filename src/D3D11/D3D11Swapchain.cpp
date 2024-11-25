#include "D3D11Swapchain.h"
#include "D3D11Utils.h"
#include "../Common.h"

namespace grabs::D3D11
{
    D3D11Swapchain::D3D11Swapchain(IDXGIFactory1* factory, ID3D11Device* device, DXGISurface* surface,
        const SwapchainDescription& description)
    {
        const DXGI_FORMAT format = Utils::FormatToD3D(description.Format);
        const UINT width = description.Size.Width;
        const UINT height = description.Size.Height;

        DXGI_SWAP_CHAIN_DESC desc
        {
            .BufferDesc = { .Width = width, .Height = height, .Format = format },
            .SampleDesc = { .Count = 1, .Quality = 0 },
            .BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            .BufferCount = description.NumBuffers,
            .OutputWindow = surface->Window,
            .Windowed = TRUE,
            .SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            .Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING | DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH
        };

        D3D11_CHECK_RESULT(factory->CreateSwapChain(device, &desc, &Swapchain));
    }

    D3D11Swapchain::~D3D11Swapchain()
    {
        Swapchain->Release();
    }

    TextureView* D3D11Swapchain::GetNextTexture()
    {
        GS_TODO
    }

    void D3D11Swapchain::Present()
    {
        GS_TODO
    }
}
