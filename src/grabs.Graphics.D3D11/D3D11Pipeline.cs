using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using grabs.Core;
using TerraFX.Interop.DirectX;
using static grabs.Graphics.D3D11.D3DResult;
using static TerraFX.Interop.DirectX.D3D11_CULL_MODE;
using static TerraFX.Interop.DirectX.D3D11_DEPTH_WRITE_MASK;
using static TerraFX.Interop.DirectX.D3D11_FILL_MODE;
using static TerraFX.Interop.DirectX.D3D11_INPUT_CLASSIFICATION;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11Pipeline : Pipeline
{
    public readonly ID3D11VertexShader* VertexShader;
    public readonly ID3D11PixelShader* PixelShader;

    public readonly ID3D11InputLayout* InputLayout;

    public readonly ID3D11DepthStencilState* DepthStencilState;
    public readonly ID3D11RasterizerState* RasterizerState;
    public readonly ID3D11BlendState* BlendState;
    public readonly Vector4 BlendConstants;

    public readonly D3D11DescriptorLayout[] Layouts;

    public readonly D3D_PRIMITIVE_TOPOLOGY PrimitiveTopology;
    
    public D3D11Pipeline(ID3D11Device* device, in PipelineDescription description)
    {
        D3D11ShaderModule vShaderModule = (D3D11ShaderModule) description.VertexShader;
        fixed (ID3D11VertexShader** vShader = &VertexShader)
        {
            CheckResult(
                device->CreateVertexShader(vShaderModule.Blob->GetBufferPointer(), vShaderModule.Blob->GetBufferSize(),
                    null, vShader), "Create vertex shader");
        }

        D3D11ShaderModule pShaderModule = (D3D11ShaderModule) description.PixelShader;
        fixed (ID3D11PixelShader** pShader = &PixelShader)
        {
            CheckResult(
                device->CreatePixelShader(pShaderModule.Blob->GetBufferPointer(), pShaderModule.Blob->GetBufferSize(),
                    null, pShader), "Create pixel shader");
        }

        // Some things don't need an input layout, for example, if the drawing occurs entirely within the shader.
        // If so, ignore and do not create the input layout.
        if (description.InputLayout?.Length > 0)
        {
            using PinnedString semanticName = new PinnedString("TEXCOORD");
            
            D3D11_INPUT_ELEMENT_DESC* elementDescs = stackalloc D3D11_INPUT_ELEMENT_DESC[description.InputLayout.Length];
            for (uint i = 0; i < description.InputLayout.Length; i++)
            {
                ref InputLayoutDescription desc = ref description.InputLayout[i];

                (D3D11_INPUT_CLASSIFICATION classification, uint step) = desc.Type switch
                {
                    InputType.PerVertex => (D3D11_INPUT_PER_VERTEX_DATA, 0u),
                    InputType.PerInstance => (D3D11_INPUT_PER_INSTANCE_DATA, 1u),
                    _ => throw new ArgumentOutOfRangeException()
                };

                elementDescs[i] = new D3D11_INPUT_ELEMENT_DESC()
                {
                    SemanticName = (sbyte*) semanticName.Handle,
                    SemanticIndex = i,
                    Format = D3D11Utils.FormatToD3D(desc.Format),
                    InputSlot = desc.Slot,
                    AlignedByteOffset = desc.Offset,
                    InputSlotClass = classification,
                    InstanceDataStepRate = step
                };
            }

            fixed (ID3D11InputLayout** inputLayout = &InputLayout)
            {
                CheckResult(
                    device->CreateInputLayout(elementDescs, (uint) description.InputLayout.Length,
                        vShaderModule.Blob->GetBufferPointer(), vShaderModule.Blob->GetBufferSize(), inputLayout),
                    "Create input layout");
            }
        }

        DepthStencilDescription depthDesc = description.DepthStencilState;
        D3D11_DEPTH_STENCIL_DESC dsDesc = new()
        {
            DepthEnable = depthDesc.DepthEnabled,
            DepthWriteMask = depthDesc.DepthWrite ? D3D11_DEPTH_WRITE_MASK_ALL : D3D11_DEPTH_WRITE_MASK_ZERO,
            DepthFunc = D3D11Utils.ComparisonFunctionToD3D(depthDesc.ComparisonFunction),
            StencilEnable = false
        };

        fixed (ID3D11DepthStencilState** dsState = &DepthStencilState)
            CheckResult(device->CreateDepthStencilState(&dsDesc, dsState), "Create depth stencil state");

        RasterizerDescription rasterizerDesc = description.RasterizerState;
        D3D11_RASTERIZER_DESC rsDesc = new()
        {
            FillMode = rasterizerDesc.FillMode == FillMode.Solid
                ? D3D11_FILL_SOLID
                : D3D11_FILL_WIREFRAME,
            CullMode = rasterizerDesc.CullFace switch
            {
                CullFace.None => D3D11_CULL_NONE,
                CullFace.Front => D3D11_CULL_FRONT,
                CullFace.Back => D3D11_CULL_BACK,
                _ => throw new ArgumentOutOfRangeException()
            },
            FrontCounterClockwise = rasterizerDesc.FrontFace == CullDirection.CounterClockwise,
            
            // In Vulkan, D3D12, scissor test is always enabled.
            ScissorEnable = true
        };

        fixed (ID3D11RasterizerState** rsState = &RasterizerState)
            CheckResult(device->CreateRasterizerState(&rsDesc, rsState), "Create rasterizer state");

        BlendDescription blendDesc = description.BlendState;
        BlendConstants = blendDesc.BlendConstants;
        D3D11_BLEND_DESC bDesc = new()
        {
            IndependentBlendEnable = blendDesc.IndependentBlending
        };

        for (int i = 0; i < blendDesc.Attachments.Length; i++)
        {
            ref BlendAttachmentDescription attachmentDesc = ref blendDesc.Attachments[i];
            ref D3D11_RENDER_TARGET_BLEND_DESC rtDesc = ref bDesc.RenderTarget[0];
            
            rtDesc.BlendEnable = attachmentDesc.Enabled;
            rtDesc.SrcBlend = D3D11Utils.BlendFactorToD3D(attachmentDesc.Source);
            rtDesc.DestBlend = D3D11Utils.BlendFactorToD3D(attachmentDesc.Destination);
            rtDesc.BlendOp = D3D11Utils.BlendOperationToD3D(attachmentDesc.BlendOperation);
            rtDesc.SrcBlendAlpha = D3D11Utils.BlendFactorToD3D(attachmentDesc.SourceAlpha);
            rtDesc.DestBlendAlpha = D3D11Utils.BlendFactorToD3D(attachmentDesc.DestinationAlpha);
            rtDesc.BlendOpAlpha = D3D11Utils.BlendOperationToD3D(attachmentDesc.AlphaBlendOperation);
            rtDesc.RenderTargetWriteMask = (byte) attachmentDesc.ColorWriteMask;
        }

        fixed (ID3D11BlendState** blendState = &BlendState)
            CheckResult(device->CreateBlendState(&bDesc, blendState), "Create blend state");

        if (description.DescriptorLayouts != null)
        {
            Layouts = new D3D11DescriptorLayout[description.DescriptorLayouts.Length];
            uint currentBindingIndex = 0;
            for (int i = 0; i < Layouts.Length; i++)
            {
                D3D11DescriptorLayout layout = (D3D11DescriptorLayout) description.DescriptorLayouts[i];
                DescriptorBindingDescription[] descriptions = new DescriptorBindingDescription[layout.Bindings.Length];

                // TODO: This logic is really stupid!
                // The SPIR-v shader transpiler does these in a specific order:
                //   - Constant buffers
                //   - Samplers
                //   - Images
                // This code needs to be adjusted to work in the same way.
                // Currently, this just takes everything in ANY order and reorders it. So if you don't put things in the
                // right order, then oops.
                // TODO: While this is a neat idea, I think the remapping logic of descriptor sets needs to be reevaluated. There must be a better way.
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
        RasterizerState->Release();
        DepthStencilState->Release();
        if (InputLayout != null)
            InputLayout->Release();
        PixelShader->Release();
        VertexShader->Release();
    }
}