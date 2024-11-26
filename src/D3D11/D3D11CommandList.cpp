#include "D3D11CommandList.h"

#include "D3D11Buffer.h"
#include "D3D11Pipeline.h"
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

    void D3D11CommandList::BeginRenderPass(const RenderPassDescription& info)
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

    void D3D11CommandList::SetViewport(const Viewport& viewport)
    {
        D3D11_VIEWPORT d3dViewport
        {
            .TopLeftX = static_cast<FLOAT>(viewport.X),
            .TopLeftY = static_cast<FLOAT>(viewport.Y),
            .Width = static_cast<FLOAT>(viewport.Width),
            .Height = static_cast<FLOAT>(viewport.Height),
            .MinDepth = viewport.MinDepth,
            .MaxDepth = viewport.MaxDepth
        };

        Context->RSSetViewports(1, &d3dViewport);
    }

    void D3D11CommandList::SetPipeline(Pipeline* pipeline)
    {
        GS_NULL_CHECK(pipeline);
        const auto d3dPipeline = dynamic_cast<D3D11Pipeline*>(pipeline);

        Context->VSSetShader(d3dPipeline->VertexShader, nullptr, 0);
        Context->PSSetShader(d3dPipeline->PixelShader, nullptr, 0);
        Context->IASetInputLayout(d3dPipeline->InputLayout);

        // TODO: Add primitive type to pipeline.
        Context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
    }

    void D3D11CommandList::SetVertexBuffer(uint32_t slot, Buffer* buffer, uint32_t stride, uint32_t offset)
    {
        GS_NULL_CHECK(buffer);
        const auto d3dBuffer = dynamic_cast<D3D11Buffer*>(buffer);

        Context->IASetVertexBuffers(slot, 1, &d3dBuffer->Buffer, &stride, &offset);
    }

    void D3D11CommandList::SetIndexBuffer(Buffer* buffer, Format format)
    {
        GS_NULL_CHECK(buffer);
        const auto d3dBuffer = dynamic_cast<D3D11Buffer*>(buffer);

        DXGI_FORMAT dxgiFormat = Utils::FormatToD3D(format);

        Context->IASetIndexBuffer(d3dBuffer->Buffer, dxgiFormat, 0);
    }

    void D3D11CommandList::DrawIndexed(uint32_t numElements)
    {
        Context->DrawIndexed(numElements, 0, 0);
    }
}
