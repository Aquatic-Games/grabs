using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D_PRIMITIVE_TOPOLOGY;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Pipeline : Pipeline
{
    public override bool IsDisposed { get; protected set; }

    public readonly ID3D11VertexShader* VertexShader;

    public readonly ID3D11PixelShader* PixelShader;

    public readonly ID3D11InputLayout* InputLayout;

    public readonly D3D_PRIMITIVE_TOPOLOGY PrimitiveTopology;

    public D3D11Pipeline(ID3D11Device* device, ref readonly GraphicsPipelineInfo info)
    {
        D3D11ShaderModule vertexModule = (D3D11ShaderModule) info.VertexShader;
        D3D11ShaderModule pixelModule = (D3D11ShaderModule) info.PixelShader;

        GrabsLog.Log("Creating vertex shader.");
        fixed (ID3D11VertexShader** shader = &VertexShader)
        {
            device->CreateVertexShader(vertexModule.Code, vertexModule.CodeLength, null, shader)
                .Check("Create vertex shader");
        }

        GrabsLog.Log("Creating pixel shader.");
        fixed (ID3D11PixelShader** shader = &PixelShader)
        {
            device->CreatePixelShader(pixelModule.Code, pixelModule.CodeLength, null, shader)
                .Check("Create pixel shader");
        }

        if (info.InputLayout.Length > 0)
        {
            D3D11_INPUT_ELEMENT_DESC* inputElements = stackalloc D3D11_INPUT_ELEMENT_DESC[info.InputLayout.Length];

            List<Utf8String> semanticStrings = [];

            for (int i = 0; i < info.InputLayout.Length; i++)
            {
                ref readonly InputElementDescription element = ref info.InputLayout[i];

                Utf8String pSemantic = element.Semantic switch
                {
                    SemanticType.TexCoord => "TEXCOORD",
                    SemanticType.Position => "POSITION",
                    SemanticType.Color => "COLOR",
                    SemanticType.Normal => "NORMAL",
                    SemanticType.Tangent => "TANGENT",
                    SemanticType.Bitangent => "BITANGENT",
                    _ => throw new ArgumentOutOfRangeException()
                };
                semanticStrings.Add(pSemantic);

                inputElements[i] = new D3D11_INPUT_ELEMENT_DESC()
                {
                    SemanticName = pSemantic,
                    SemanticIndex = element.SemanticIndex,
                    Format = element.Format.ToD3D(),
                    AlignedByteOffset = element.Offset
                };
            }

            GrabsLog.Log("Creating input layout.");
            fixed (ID3D11InputLayout** layout = &InputLayout)
            {
                device->CreateInputLayout(inputElements, (uint) info.InputLayout.Length, vertexModule.Code,
                    vertexModule.CodeLength, layout).Check("Create input layout");
            }
        }

        PrimitiveTopology = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        if (InputLayout != null)
            InputLayout->Release();
        
        PixelShader->Release();
        VertexShader->Release();
    }
}