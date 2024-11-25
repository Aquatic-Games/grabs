#pragma once

#include "grabs/Swapchain.h"

#include <d3d11.h>

namespace grabs::D3D11
{
    class D3D11Swapchain : public Swapchain
    {
    public:
        D3D11Swapchain(ID3D11Device);
        ~D3D11Swapchain() override;

        TextureView* GetNextTexture() override;

        void Present() override;
    };
}
