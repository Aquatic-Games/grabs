#pragma once

#include <d3d11.h>

#include "grabs/ShaderModule.h"

namespace grabs::D3D11
{
    class D3D11ShaderModule final : public ShaderModule
    {
    public:
        ID3DBlob* Blob{};

        explicit D3D11ShaderModule(const ShaderModuleDescription& description);
        ~D3D11ShaderModule() override;
    };
}
