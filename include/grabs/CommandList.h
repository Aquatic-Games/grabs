#pragma once

#include <cstdint>

#include "Texture.h"
#include "Buffer.h"
#include "Common.h"
#include "Pipeline.h"

namespace grabs
{
    struct RenderPassDescription
    {
        Texture* Texture;
        float ClearColor[4];
    };

    class CommandList
    {
    public:
        virtual ~CommandList() = default;

        virtual void Begin() = 0;
        virtual void End() = 0;

        virtual void BeginRenderPass(const RenderPassDescription& description) = 0;
        virtual void EndRenderPass() = 0;

        virtual void SetViewport(const Viewport& viewport) = 0;

        virtual void SetPipeline(Pipeline* pipeline) = 0;

        virtual void SetVertexBuffer(uint32_t slot, Buffer* buffer, uint32_t stride, uint32_t offset) = 0;
        virtual void SetIndexBuffer(Buffer* buffer, Format format) = 0;

        virtual void DrawIndexed(uint32_t numElements) = 0;
    };
}
