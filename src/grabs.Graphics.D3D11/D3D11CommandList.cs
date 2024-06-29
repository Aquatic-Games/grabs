using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace grabs.Graphics.D3D11;

public sealed class D3D11CommandList : CommandList
{
    private D3D11Pipeline _currentPipeline;
    private D3D11DescriptorSet[] _setSets; // Lol
    
    public ID3D11DeviceContext Context;
    public ID3D11CommandList CommandList;

    public D3D11CommandList(ID3D11Device device)
    {
        // TODO: This is temporary, set to an arbitrary value. This should auto expand to support thousands of sets if needed.
        _setSets = new D3D11DescriptorSet[16];
        
        Context = device.CreateDeferredContext();
    }

    public override void Begin()
    {
        CommandList?.Dispose();
        CommandList = null;
    }

    public override void End()
    {
        CommandList = Context.FinishCommandList(false);
    }

    public override void BeginRenderPass(in RenderPassDescription description)
    {
        D3D11Framebuffer framebuffer = (D3D11Framebuffer) description.Framebuffer;
        
        Context.OMSetRenderTargets(framebuffer.RenderTargets, framebuffer.DepthTarget);

        if (description.ColorLoadOp is LoadOp.Clear)
        {
            for (int i = 0; i < framebuffer.RenderTargets.Length; i++)
                Context.ClearRenderTargetView(framebuffer.RenderTargets[i], new Color4(description.ClearColor));
        }

        if (framebuffer.DepthTarget != null)
        {
            DepthStencilClearFlags dscf = DepthStencilClearFlags.None;
            if (description.DepthLoadOp == LoadOp.Clear)
                dscf |= DepthStencilClearFlags.Depth;
            if (description.StencilLoadOp == LoadOp.Clear)
                dscf |= DepthStencilClearFlags.Stencil;

            Context.ClearDepthStencilView(framebuffer.DepthTarget, dscf, description.DepthValue,
                description.StencilValue);
        }
    }

    public override void EndRenderPass() { }

    public override unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;

        if (d3dBuffer.Description.Dynamic)
        {
            MappedSubresource mResource = Context.Map(d3dBuffer.Buffer, MapMode.WriteDiscard);
            Unsafe.CopyBlock((byte*) mResource.DataPointer + offsetInBytes, pData, sizeInBytes);
            Context.Unmap(d3dBuffer.Buffer);
        }
        else
        {
            Context.UpdateSubresource(d3dBuffer.Buffer, 0,
                new Box((int) offsetInBytes, 0, 0, (int) (offsetInBytes + sizeInBytes), 1, 1), (nint) pData, 0, 0);
        }
    }

    public override void GenerateMipmaps(Texture texture)
    {
        Context.GenerateMips(((D3D11Texture) texture).ResourceView);
    }

    public override void SetViewport(in Viewport viewport)
    {
        Context.RSSetViewport(viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinDepth,
            viewport.MaxDepth);
    }

    public override unsafe void SetPipeline(Pipeline pipeline)
    {
        D3D11Pipeline d3dPipeline = (D3D11Pipeline) pipeline;
        _currentPipeline = d3dPipeline;

        Vector4 blendConstants = d3dPipeline.BlendConstants;
        
        Context.VSSetShader(d3dPipeline.VertexShader);
        Context.PSSetShader(d3dPipeline.PixelShader);
        if (d3dPipeline.InputLayout != null)
            Context.IASetInputLayout(d3dPipeline.InputLayout);
        Context.OMSetDepthStencilState(d3dPipeline.DepthStencilState);
        Context.RSSetState(d3dPipeline.RasterizerState);
        Context.OMSetBlendState(d3dPipeline.BlendState, &blendConstants.X);
        Context.IASetPrimitiveTopology(d3dPipeline.PrimitiveTopology);
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset)
    {
        Context.IASetVertexBuffer((int) slot, ((D3D11Buffer) buffer).Buffer, (int) stride, (int) offset);
    }

    public override void SetIndexBuffer(Buffer buffer, Format format)
    {
        Context.IASetIndexBuffer(((D3D11Buffer) buffer).Buffer, format.ToDXGIFormat(), 0);
    }

    public override void SetDescriptorSet(uint index, DescriptorSet set)
    {
        D3D11DescriptorSet d3dSet = (D3D11DescriptorSet) set;
        _setSets[index] = d3dSet;
    }

    public override void Draw(uint numVertices)
    {
        SetPreDrawParameters();
        Context.Draw((int) numVertices, 0);
    }

    public override void DrawIndexed(uint numIndices)
    {
        SetPreDrawParameters();
        Context.DrawIndexed((int) numIndices, 0, 0);
    }

    public override void DrawIndexed(uint numIndices, uint startIndex, int baseVertex)
    {
        SetPreDrawParameters();
        Context.DrawIndexed((int) numIndices, (int) startIndex, baseVertex);
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
                    
                        if ((binding.Stages & ShaderStage.Vertex) == ShaderStage.Vertex)
                            Context.VSSetConstantBuffer((int) binding.Binding, buffer.Buffer);
                        if ((binding.Stages & ShaderStage.Pixel) == ShaderStage.Pixel)
                            Context.PSSetConstantBuffer((int) binding.Binding, buffer.Buffer);
                        if ((binding.Stages & ShaderStage.Compute) == ShaderStage.Compute)
                            throw new NotImplementedException();
                        
                        break;
                
                    case DescriptorType.Texture:
                        D3D11Texture texture = (D3D11Texture) desc.Texture;
                    
                        if ((binding.Stages & ShaderStage.Vertex) == ShaderStage.Vertex)
                            Context.VSSetShaderResource((int) binding.Binding, texture.ResourceView);
                        if ((binding.Stages & ShaderStage.Pixel) == ShaderStage.Pixel)
                            Context.PSSetShaderResource((int) binding.Binding, texture.ResourceView);
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
        CommandList?.Dispose();
        Context.Dispose();
    }
}