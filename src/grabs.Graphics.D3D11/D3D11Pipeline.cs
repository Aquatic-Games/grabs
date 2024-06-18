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
    public readonly ID3D11RasterizerState RasterizerState;

    public readonly D3D11DescriptorLayout[] Layouts;

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

        DepthStencilDescription depthDesc = description.DepthStencilState;
        Vortice.Direct3D11.DepthStencilDescription dsDesc = new Vortice.Direct3D11.DepthStencilDescription()
        {
            DepthEnable = depthDesc.DepthEnabled,
            DepthWriteMask = depthDesc.DepthWrite ? DepthWriteMask.All : DepthWriteMask.Zero,
            DepthFunc = depthDesc.ComparisonFunction.ToComparisonFunc(),
            StencilEnable = false
        };

        DepthStencilState = device.CreateDepthStencilState(dsDesc);

        RasterizerDescription rasterizerDesc = description.RasterizerState;
        Vortice.Direct3D11.RasterizerDescription rsDesc = new Vortice.Direct3D11.RasterizerDescription()
        {
            FillMode = rasterizerDesc.FillMode == FillMode.Solid
                ? Vortice.Direct3D11.FillMode.Solid
                : Vortice.Direct3D11.FillMode.Wireframe,
            CullMode = rasterizerDesc.CullFace switch
            {
                CullFace.None => CullMode.None,
                CullFace.Front => CullMode.Front,
                CullFace.Back => CullMode.Back,
                _ => throw new ArgumentOutOfRangeException()
            },
            FrontCounterClockwise = rasterizerDesc.FrontFace == CullDirection.CounterClockwise
        };

        RasterizerState = device.CreateRasterizerState(rsDesc);

        Layouts = new D3D11DescriptorLayout[description.DescriptorLayouts.Length];
        uint currentBindingIndex = 0;
        for (int i = 0; i < Layouts.Length; i++)
        {
            D3D11DescriptorLayout layout = (D3D11DescriptorLayout) description.DescriptorLayouts[i];
            DescriptorBindingDescription[] descriptions = new DescriptorBindingDescription[layout.Bindings.Length];

            for (int j = 0; j < descriptions.Length; j++)
            {
                DescriptorBindingDescription desc = layout.Bindings[j];
                desc.Binding = currentBindingIndex++;
                descriptions[j] = desc;
            }

            Layouts[i] = new D3D11DescriptorLayout(descriptions);
        }

        PrimitiveTopology = description.PrimitiveType.ToPrimitiveTopology();
    }
    
    public override void Dispose()
    {
        RasterizerState.Dispose();
        DepthStencilState.Dispose();
        InputLayout.Dispose();
        PixelShader.Dispose();
        VertexShader.Dispose();
    }
}