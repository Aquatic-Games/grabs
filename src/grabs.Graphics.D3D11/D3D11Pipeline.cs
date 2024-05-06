using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Pipeline : Pipeline
{
    public readonly ID3D11VertexShader VertexShader;
    public readonly ID3D11PixelShader PixelShader;

    public readonly ID3D11InputLayout InputLayout;

    public readonly ID3D11DepthStencilState DepthStencilState;

    public readonly PrimitiveTopology PrimitiveTopology;
    
    public D3D11Pipeline(ID3D11Device device, in PipelineDescription description)
    {
        D3D11ShaderModule vShaderModule = (D3D11ShaderModule) description.VertexShader;
        VertexShader = device.CreateVertexShader(vShaderModule.Blob);

        D3D11ShaderModule pShaderModule = (D3D11ShaderModule) description.PixelShader;
        PixelShader = device.CreatePixelShader(pShaderModule.Blob);

        InputElementDescription[] elementDescs = new InputElementDescription[description.InputLayout.Length];
        for (int i = 0; i < description.InputLayout.Length; i++)
        {
            ref InputLayoutDescription desc = ref description.InputLayout[i];

            (InputClassification classification, int step) = desc.Type switch
            {
                InputType.PerVertex => (InputClassification.PerVertexData, 0),
                InputType.PerInstance => (InputClassification.PerInstanceData, 1),
                _ => throw new ArgumentOutOfRangeException()
            };

            elementDescs[i] = new InputElementDescription("TEXCOORD", i, desc.Format.ToDXGIFormat(), (int) desc.Offset,
                (int) desc.Slot, classification, step);
        }

        InputLayout = device.CreateInputLayout(elementDescs, vShaderModule.Blob);

        DepthStencilDescription depthDesc = description.DepthStencilDescription;
        
        Vortice.Direct3D11.DepthStencilDescription dsDesc = new Vortice.Direct3D11.DepthStencilDescription()
        {
            DepthEnable = depthDesc.DepthEnabled,
            DepthWriteMask = depthDesc.DepthWrite ? DepthWriteMask.All : DepthWriteMask.Zero,
            DepthFunc = depthDesc.ComparisonFunction.ToComparisonFunc(),
            StencilEnable = false
        };

        DepthStencilState = device.CreateDepthStencilState(dsDesc);

        PrimitiveTopology = description.PrimitiveType.ToPrimitiveTopology();
    }
    
    public override void Dispose()
    {
        VertexShader.Dispose();
        PixelShader.Dispose();
        InputLayout.Dispose();
    }
}