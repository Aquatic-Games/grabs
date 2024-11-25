#include "D3D11Texture.h"

#include "../Common.h"

namespace grabs::D3D11
{
    D3D11Texture::D3D11Texture(ID3D11Resource* texture, ID3D11RenderTargetView* target)
    {
        Texture = texture;
        RenderTarget = target;
    }

    D3D11Texture::~D3D11Texture()
    {
        if (DepthTarget)
            DepthTarget->Release();
        if (RenderTarget)
            RenderTarget->Release();
        if (TextureSrv)
            TextureSrv->Release();

        Texture->Release();
    }

    Size3D D3D11Texture::Size() const
    {
        GS_TODO
    }

    grabs::Format D3D11Texture::Format() const
    {
        GS_TODO
    }
}
