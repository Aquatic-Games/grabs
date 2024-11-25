#pragma once

#include "Texture.h"

namespace grabs
{
    struct RenderPassInfo
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

        virtual void BeginRenderPass(const RenderPassInfo& info) = 0;
        virtual void EndRenderPass() = 0;
    };
}
