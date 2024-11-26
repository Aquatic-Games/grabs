#pragma once

#include <d3d11.h>

#include "grabs/CommandList.h"

namespace grabs::D3D11
{
    class D3D11CommandList final : public CommandList
    {
    public:
        ID3D11DeviceContext* Context{};
        ID3D11CommandList* CommandList{};

        explicit D3D11CommandList(ID3D11Device* device);
        ~D3D11CommandList() override;

        void Begin() override;
        void End() override;

        void BeginRenderPass(const RenderPassDescription& info) override;
        void EndRenderPass() override;

        void SetViewport(const Viewport& viewport) override;

        void SetPipeline(Pipeline* pipeline) override;

        void SetVertexBuffer(uint32_t slot, Buffer* buffer, uint32_t stride, uint32_t offset) override;
        void SetIndexBuffer(Buffer* buffer, Format format) override;

        void DrawIndexed(uint32_t numElements) override;
    };
}
