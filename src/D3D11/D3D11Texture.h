#pragma once

#include <d3d11.h>

#include "grabs/Texture.h"

namespace grabs::D3D11
{
    class D3D11Texture final : public Texture
    {
    public:
        ID3D11Resource* Texture{};
        ID3D11ShaderResourceView* TextureSrv{};

        ID3D11RenderTargetView* RenderTarget{};
        ID3D11DepthStencilView* DepthTarget{};

        // Used for swapchain texture.
        D3D11Texture(ID3D11Resource* texture, ID3D11RenderTargetView* target);
        ~D3D11Texture() override;

        [[nodiscard]] Size3D Size() const override;
        [[nodiscard]] grabs::Format Format() const override;
    };
}
