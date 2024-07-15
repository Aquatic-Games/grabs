using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static grabs.Graphics.D3D11.D3DResult;
using static TerraFX.Interop.DirectX.D3D11_CLEAR_FLAG;
using static TerraFX.Interop.DirectX.D3D11_MAP;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11CommandList : CommandList
{
    private D3D11Pipeline _currentPipeline;
    private D3D11DescriptorSet[] _setSets; // Lol
    
    public ID3D11DeviceContext* Context;
    public ID3D11CommandList* CommandList;

    public D3D11CommandList(ID3D11Device* device)
    {
        // TODO: This is temporary, set to an arbitrary value. This should auto expand to support thousands of sets if needed.
        _setSets = new D3D11DescriptorSet[16];
        
        fixed (ID3D11DeviceContext** context = &Context)
            CheckResult(device->CreateDeferredContext(0, context), "Create deferred context");
    }

    public override void Begin()
    {
        if (CommandList != null)
            CommandList->Release();
        CommandList = null;
    }

    public override void End()
    {
        fixed (ID3D11CommandList** commandList = &CommandList)
            CheckResult(Context->FinishCommandList(false, commandList));
    }

    public override void BeginRenderPass(in RenderPassDescription description)
    {
        D3D11Framebuffer framebuffer = (D3D11Framebuffer) description.Framebuffer;
        
        fixed (ID3D11RenderTargetView** renderTargets = framebuffer.RenderTargets) 
            Context->OMSetRenderTargets((uint) framebuffer.RenderTargets.Length, renderTargets, framebuffer.DepthTarget);

        if (description.ColorLoadOp is LoadOp.Clear)
        {
            float* clearColor = stackalloc float[4]
            {
                description.ClearColor.X, description.ClearColor.Y, description.ClearColor.Z, description.ClearColor.W
            };
            
            for (int i = 0; i < framebuffer.RenderTargets.Length; i++)
                Context->ClearRenderTargetView(framebuffer.RenderTargets[i], clearColor);
        }

        if (framebuffer.DepthTarget != null)
        {
            D3D11_CLEAR_FLAG clearFlags = 0;
            if (description.DepthLoadOp == LoadOp.Clear)
                clearFlags |= D3D11_CLEAR_DEPTH;
            if (description.StencilLoadOp == LoadOp.Clear)
                clearFlags |= D3D11_CLEAR_STENCIL;

            Context->ClearDepthStencilView(framebuffer.DepthTarget, (uint) clearFlags, description.DepthValue,
                description.StencilValue);
        }
    }

    public override void EndRenderPass() { }

    public override void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;

        // TODO: If offsetInBytes != 0, you can't map the buffer. For now I've just disabled the offset entirely.
        if (d3dBuffer.Description.Dynamic)
        {
            if (offsetInBytes != 0)
            {
                throw new NotImplementedException(
                    "Cannot currently update a dynamic buffer with an offset of anything other than 0.");
            }

            D3D11_MAPPED_SUBRESOURCE mResource;
            CheckResult(Context->Map((ID3D11Resource*) d3dBuffer.Buffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mResource),
                "Map buffer");
            Unsafe.CopyBlock((byte*) mResource.pData + offsetInBytes, pData, sizeInBytes);
            Context->Unmap((ID3D11Resource*) d3dBuffer.Buffer, 0);
        }
        else
        {
            D3D11_BOX box = new D3D11_BOX((int) offsetInBytes, 0, 0, (int) (offsetInBytes + sizeInBytes), 1, 1);
            Context->UpdateSubresource((ID3D11Resource*) d3dBuffer.Buffer, 0, &box, pData, 0, 0);
        }
    }

    public override void GenerateMipmaps(Texture texture)
    {
        Context->GenerateMips(((D3D11Texture) texture).ResourceView);
    }

    public override void SetViewport(in Viewport viewport)
    {
        D3D11_VIEWPORT vp = new D3D11_VIEWPORT(viewport.X, viewport.Y, viewport.Width, viewport.Height,
            viewport.MinDepth, viewport.MaxDepth);
        Context->RSSetViewports(1, &vp);
    }

    public override void SetScissor(in Rectangle rectangle)
    {
        RECT rect = new RECT(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        Context->RSSetScissorRects(1, &rect);
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        _currentPipeline = d3dPipeline;

        Vector4 blendConstants = d3dPipeline.BlendConstants;
        
        Context->VSSetShader(d3dPipeline.VertexShader, null, 0);
        Context->PSSetShader(d3dPipeline.PixelShader, null, 0);
        if (d3dPipeline.InputLayout != null)
            Context->IASetInputLayout(d3dPipeline.InputLayout);
        Context->OMSetDepthStencilState(d3dPipeline.DepthStencilState, 0);
        Context->RSSetState(d3dPipeline.RasterizerState);
        Context->OMSetBlendState(d3dPipeline.BlendState, &blendConstants.X, 0xFFFFFF);
        Context->IASetPrimitiveTopology(d3dPipeline.PrimitiveTopology);
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset)
    {
        ID3D11Buffer* buf = ((D3D11Buffer) buffer).Buffer;
        Context->IASetVertexBuffers(slot, 1, &buf, &stride, &offset);
    }

    public override void SetIndexBuffer(Buffer buffer, Format format)
    {
        Context->IASetIndexBuffer(((D3D11Buffer) buffer).Buffer, D3D11Utils.FormatToD3D(format), 0);
    }

    public override void SetDescriptorSet(uint index, DescriptorSet set)
    {
        D3D11DescriptorSet d3dSet = (D3D11DescriptorSet) set;
        _setSets[index] = d3dSet;
    }

    public override void Draw(uint numVertices)
    {
        SetPreDrawParameters();
        Context->Draw(numVertices, 0);
    }

    public override void DrawIndexed(uint numIndices)
    {
        SetPreDrawParameters();
        Context->DrawIndexed(numIndices, 0, 0);
    }

    public override void DrawIndexed(uint numIndices, uint startIndex, int baseVertex)
    {
        SetPreDrawParameters();
        Context->DrawIndexed(numIndices, startIndex, baseVertex);
    }

    private void SetPreDrawParameters()
    {
        if (_currentPipeline.Layouts == null)
            return;
        
        for (int i = 0; i < _currentPipeline.Layouts.Length; i++)
        {
            D3D11DescriptorLayout layout = _currentPipeline.Layouts[i];

            for (int j = 0; j < layout.Bindings.Length; j++)
            {
                ref DescriptorBindingDescription binding = ref layout.Bindings[j];
                ref DescriptorSetDescription desc = ref _setSets[i].Descriptions[j];

                switch (binding.Type)
                {
                    case DescriptorType.ConstantBuffer:
                        D3D11Buffer buffer = (D3D11Buffer) desc.Buffer;
                        ID3D11Buffer* buf = buffer.Buffer;
                    
                        if ((binding.Stages & ShaderStage.Vertex) == ShaderStage.Vertex)
                            Context->VSSetConstantBuffers(binding.Binding, 1, &buf);
                        if ((binding.Stages & ShaderStage.Pixel) == ShaderStage.Pixel)
                            Context->PSSetConstantBuffers(binding.Binding, 1, &buf);
                        if ((binding.Stages & ShaderStage.Compute) == ShaderStage.Compute)
                            throw new NotImplementedException();
                        
                        break;
                
                    case DescriptorType.Texture:
                        D3D11Texture texture = (D3D11Texture) desc.Texture;
                        ID3D11ShaderResourceView* resView = texture.ResourceView;
                    
                        if ((binding.Stages & ShaderStage.Vertex) == ShaderStage.Vertex)
                            Context->VSSetShaderResources(binding.Binding, 1, &resView);
                        if ((binding.Stages & ShaderStage.Pixel) == ShaderStage.Pixel)
                            Context->PSSetShaderResources(binding.Binding, 1, &resView);
                        if ((binding.Stages & ShaderStage.Compute) == ShaderStage.Compute)
                            throw new NotImplementedException();
                    
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public override void Dispose()
    {
        if (CommandList != null)
            CommandList->Release();

        Context->Release();
    }
}