#include "D3D11Pipeline.h"
#include "D3D11ShaderModule.h"
#include "D3D11Utils.h"

namespace grabs::D3D11
{
    D3D11Pipeline::D3D11Pipeline(ID3D11Device* device, const PipelineDescription& description)
    {
        auto vertexModule = dynamic_cast<D3D11ShaderModule*>(description.VertexShader);
        auto vertData = vertexModule->Blob->GetBufferPointer();
        auto vertSize = vertexModule->Blob->GetBufferSize();

        D3D11_CHECK_RESULT(device->CreateVertexShader(vertData, vertSize,nullptr, &VertexShader));

        auto pixelModule = dynamic_cast<D3D11ShaderModule*>(description.PixelShader);
        auto pixlData = pixelModule->Blob->GetBufferPointer();
        auto pixlSize = pixelModule->Blob->GetBufferSize();

        D3D11_CHECK_RESULT(device->CreatePixelShader(pixlData, pixlSize,nullptr, &PixelShader));

        std::vector<D3D11_INPUT_ELEMENT_DESC> elementDescs;
        elementDescs.reserve(description.InputLayout.size());

        uint32_t i = 0;
        for (const auto& layout : description.InputLayout)
        {
            D3D11_INPUT_ELEMENT_DESC desc
            {
                .SemanticName = "TEXCOORD",
                .SemanticIndex = i++,
                .Format = Utils::FormatToD3D(layout.Format),
                .InputSlot = layout.Slot,
                .AlignedByteOffset = layout.Offset,
                .InputSlotClass = layout.Type == InputType::PerInstance ? D3D11_INPUT_PER_INSTANCE_DATA : D3D11_INPUT_PER_VERTEX_DATA,
                .InstanceDataStepRate = 0
            };

            elementDescs.push_back(desc);
        }

        D3D11_CHECK_RESULT(device->CreateInputLayout(elementDescs.data(), elementDescs.size(), vertData, vertSize, &InputLayout));
    }

    D3D11Pipeline::~D3D11Pipeline()
    {
        InputLayout->Release();
        PixelShader->Release();
        VertexShader->Release();
    }
}
