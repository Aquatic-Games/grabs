#include "D3D11CommandList.h"

#include "D3D11Texture.h"
#include "D3D11Utils.h"
#include "../Common.h"

namespace grabs::D3D11
{
    D3D11CommandList::D3D11CommandList(ID3D11Device* device)
    {
        D3D11_CHECK_RESULT(device->CreateDeferredContext(0, &Context));
    }

    D3D11CommandList::~D3D11CommandList()
    {
        if (CommandList)
            CommandList->Release();

        Context->Release();
    }

    void D3D11CommandList::Begin()
    {
        if (CommandList)
            CommandList->Release();

        CommandList = nullptr;
    }

    void D3D11CommandList::End()
    {
        D3D11_CHECK_RESULT(Context->FinishCommandList(FALSE, &CommandList));
    }

    void D3D11CommandList::BeginRenderPass(const RenderPassInfo& info)
    {
        GS_NULL_CHECK(info.Texture);
        auto d3dTexture = dynamic_cast<D3D11Texture*>(info.Texture);
        GS_NULL_CHECK(d3dTexture->RenderTarget);

        Context->OMSetRenderTargets(1, &d3dTexture->RenderTarget, nullptr);
        Context->ClearRenderTargetView(d3dTexture->RenderTarget, info.ClearColor);
    }

    void D3D11CommandList::EndRenderPass()
    {
        // Does nothing.
    }
}
