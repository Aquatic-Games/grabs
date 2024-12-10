using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using static grabs.Graphics.D3D11.D3D11Result;
using static TerraFX.Interop.DirectX.D3D_PRIMITIVE_TOPOLOGY;
using static TerraFX.Interop.DirectX.D3D11_INPUT_CLASSIFICATION;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Pipeline : Pipeline
{
    public ID3D11VertexShader* VertexShader;

    public ID3D11PixelShader* PixelShader;

    public ID3D11InputLayout* InputLayout;

    public D3D_PRIMITIVE_TOPOLOGY PrimitiveTopology;

    public D3D11Pipeline(ID3D11Device* device, in PipelineDescription description)
    {
        D3D11ShaderModule vertexModule = (D3D11ShaderModule) description.VertexShader;
        void* vertexData = vertexModule.Blob->GetBufferPointer();
        nuint vertexDataSize = vertexModule.Blob->GetBufferSize();
        
        fixed (ID3D11VertexShader** vertexShader = &VertexShader)
        {
            CheckResult(device->CreateVertexShader(vertexData, vertexDataSize, null, vertexShader),
                "Create vertex shader");
        }

        fixed (ID3D11PixelShader** pixelShader = &PixelShader)
        {
            D3D11ShaderModule pixelModule = (D3D11ShaderModule) description.PixelShader;
            void* pixelData = pixelModule.Blob->GetBufferPointer();
            nuint pixelDataSize = pixelModule.Blob->GetBufferSize();

            CheckResult(device->CreatePixelShader(pixelData, pixelDataSize, null, pixelShader), "Create pixel shader");
        }

        int numInputLayouts = description.InputLayout.Length;
        D3D11_INPUT_ELEMENT_DESC* descs = stackalloc D3D11_INPUT_ELEMENT_DESC[numInputLayouts];

        using PinnedString semanticName = new PinnedString("TEXCOORD");
        
        for (int i = 0; i < numInputLayouts; i++)
        {
            ref readonly InputLayoutDescription layout = ref description.InputLayout[i];

            descs[i] = new D3D11_INPUT_ELEMENT_DESC
            {
                SemanticName = semanticName,
                SemanticIndex = (uint) i,
                
                Format = layout.Format.ToD3D(),
                AlignedByteOffset = layout.Offset,
                InputSlot = layout.Slot,
                InputSlotClass = layout.Type == InputType.PerVertex
                    ? D3D11_INPUT_PER_VERTEX_DATA
                    : D3D11_INPUT_PER_INSTANCE_DATA
            };
        }

        fixed (ID3D11InputLayout** inputLayout = &InputLayout)
        {
            CheckResult(
                device->CreateInputLayout(descs, (uint) numInputLayouts, vertexData, vertexDataSize, inputLayout),
                "Create input layout");
        }

        PrimitiveTopology = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
    }
    
    public override void Dispose()
    {
        InputLayout->Release();
        PixelShader->Release();
        VertexShader->Release();
    }
}