#pragma once

#include "Texture.h"

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
    };
}
