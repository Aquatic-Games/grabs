#pragma once

#include <d3d11.h>

#include "grabs/Buffer.h"

namespace grabs::D3D11
{
    class D3D11Buffer final : public Buffer
    {
    public:
        ID3D11Buffer* Buffer{};

        D3D11Buffer(ID3D11Device* device, const BufferDescription& description, void* data);
        ~D3D11Buffer() override;
    };
}
