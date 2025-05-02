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

        PrimitiveTopology = D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        PixelShader->Release();
        VertexShader->Release();
    }
}