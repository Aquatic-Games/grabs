#pragma once

#include <d3d11.h>

#include "grabs/Pipeline.h"

namespace grabs::D3D11
{
    class D3D11Pipeline final : public Pipeline
    {
    public:
        ID3D11VertexShader* VertexShader{};
        ID3D11PixelShader* PixelShader{};

        ID3D11InputLayout* InputLayout{};

        D3D11Pipeline(ID3D11Device* device, const PipelineDescription& description);
        ~D3D11Pipeline() override;
    };
}
