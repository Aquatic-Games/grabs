#include "D3D11Buffer.h"

#include "D3D11Utils.h"

namespace grabs::D3D11
{
    D3D11Buffer::D3D11Buffer(ID3D11Device* device, const BufferDescription& description, void* data)
    {
        UINT bindFlags = 0;
        switch (description.Type)
        {
            case BufferType::Vertex:
                bindFlags = D3D11_BIND_VERTEX_BUFFER;
                break;
            case BufferType::Index:
                bindFlags = D3D11_BIND_INDEX_BUFFER;
                break;
            case BufferType::Constant:
                bindFlags = D3D11_BIND_CONSTANT_BUFFER;
                break;
        }

        const D3D11_BUFFER_DESC desc
        {
            .ByteWidth = description.Size,
            .Usage = description.Dynamic ? D3D11_USAGE_DYNAMIC : D3D11_USAGE_DEFAULT,
            .BindFlags = bindFlags,
            .CPUAccessFlags = 0
        };

        const D3D11_SUBRESOURCE_DATA subData { .pSysMem = data };
        D3D11_CHECK_RESULT(device->CreateBuffer(&desc, data ? &subData : nullptr, &Buffer));
    }

    D3D11Buffer::~D3D11Buffer()
    {
        Buffer->Release();
    }
}
