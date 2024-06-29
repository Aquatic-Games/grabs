using System;
using System.Numerics;
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
    public readonly ID3D11BlendState BlendState;
    public readonly Vector4 BlendConstants;

    public readonly D3D11DescriptorLayout[] Layouts;

    public readonly PrimitiveTopology PrimitiveTopology;
    
    public D3D11Pipeline(ID3D11Device device, in PipelineDescription description)
    {
        D3D11ShaderModule vShaderModule = (D3D11ShaderModule) description.VertexShader;
        VertexShader = device.CreateVertexShader(vShaderModule.Blob);

        D3D11ShaderModule pShaderModule = (D3D11ShaderModule) description.PixelShader;
        PixelShader = device.CreatePixelShader(pShaderModule.Blob);

        // Some things don't need an input layout, for example, if the drawing occurs entirely within the shader.
        // If so, ignore and do not create the input layout.
        if (description.InputLayout?.Length > 0)
        {
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

                elementDescs[i] = new InputElementDescription("TEXCOORD", i, D3D11Utils.FormatToD3D(desc.Format),
                    (int) desc.Offset, (int) desc.Slot, classification, step);
            }

            InputLayout = device.CreateInputLayout(elementDescs, vShaderModule.Blob);
        }

        DepthStencilDescription depthDesc = description.DepthStencilState;
        Vortice.Direct3D11.DepthStencilDescription dsDesc = new Vortice.Direct3D11.DepthStencilDescription()
        {
            DepthEnable = depthDesc.DepthEnabled,
            DepthWriteMask = depthDesc.DepthWrite ? DepthWriteMask.All : DepthWriteMask.Zero,
            DepthFunc = D3D11Utils.ComparisonFunctionToD3D(depthDesc.ComparisonFunction),
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
            FrontCounterClockwise = rasterizerDesc.FrontFace == CullDirection.CounterClockwise,
            
            // In Vulkan, D3D12, scissor test is always enabled.
            ScissorEnable = true
        };

        RasterizerState = device.CreateRasterizerState(rsDesc);

        BlendDescription blendDesc = description.BlendState;
        BlendConstants = blendDesc.BlendConstants;
        Vortice.Direct3D11.BlendDescription bDesc = new Vortice.Direct3D11.BlendDescription()
        {
            IndependentBlendEnable = blendDesc.IndependentBlending
        };

        for (int i = 0; i < blendDesc.Attachments.Length; i++)
        {
            ref BlendAttachmentDescription attachmentDesc = ref blendDesc.Attachments[i];
            ref RenderTargetBlendDescription rtDesc = ref bDesc.RenderTarget[i];
            
            rtDesc.BlendEnable = attachmentDesc.Enabled;
            rtDesc.SourceBlend = D3D11Utils.BlendFactorToD3D(attachmentDesc.Source);
            rtDesc.DestinationBlend = D3D11Utils.BlendFactorToD3D(attachmentDesc.Destination);
            rtDesc.BlendOperation = D3D11Utils.BlendOperationToD3D(attachmentDesc.BlendOperation);
            rtDesc.SourceBlendAlpha = D3D11Utils.BlendFactorToD3D(attachmentDesc.SourceAlpha);
            rtDesc.DestinationBlendAlpha = D3D11Utils.BlendFactorToD3D(attachmentDesc.DestinationAlpha);
            rtDesc.BlendOperationAlpha = D3D11Utils.BlendOperationToD3D(attachmentDesc.AlphaBlendOperation);
            rtDesc.RenderTargetWriteMask = (ColorWriteEnable) attachmentDesc.ColorWriteMask;
        }

        BlendState = device.CreateBlendState(bDesc);

        if (description.DescriptorLayouts != null)
        {
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
        }

        PrimitiveTopology = D3D11Utils.PrimitiveTypeToD3D(description.PrimitiveType);
    }
    
    public override void Dispose()
    {
        RasterizerState.Dispose();
        DepthStencilState.Dispose();
        InputLayout?.Dispose();
        PixelShader.Dispose();
        VertexShader.Dispose();
    }
}