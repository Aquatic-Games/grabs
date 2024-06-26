﻿using System;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Device : Device
{
    private readonly IDXGIFactory _factory;
    private readonly D3D11Surface _surface;

    private D3D11Swapchain _swapchain;
    
    public readonly ID3D11Device Device;
    public readonly ID3D11DeviceContext Context;
    
    public D3D11Device(IDXGIFactory1 factory, D3D11Surface surface, IDXGIAdapter1 adapter)
    {
        _factory = factory;
        _surface = surface;
        
        FeatureLevel[] levels = new[]
        {
            FeatureLevel.Level_11_0
        };

        Result result;
        if ((result = Vortice.Direct3D11.D3D11.D3D11CreateDevice(adapter, DriverType.Unknown,
                DeviceCreationFlags.BgraSupport, levels, out Device, out Context)).Failure)
        {
            throw new Exception($"Failed to create D3D11 device: {result.Description}");
        }
    }

    public override Swapchain CreateSwapchain(in SwapchainDescription description)
    {
        // TODO: Support multiple swapchains.
        _swapchain = new D3D11Swapchain(_factory, Device, Context, _surface, description);
        return _swapchain;
    }

    public override CommandList CreateCommandList()
    {
        D3D11CommandList commandList = new D3D11CommandList(Device);
        _swapchain.CommandLists.Add(commandList);
        return commandList;
    }

    public override Pipeline CreatePipeline(in PipelineDescription description)
    {
        return new D3D11Pipeline(Device, description);
    }

    public override unsafe Buffer CreateBuffer(in BufferDescription description, void* pData)
    {
        return new D3D11Buffer(Device, description, pData);
    }

    public override unsafe Texture CreateTexture(in TextureDescription description, void** ppData)
    {
        return new D3D11Texture(Device, Context, description, ppData);
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        return new D3D11ShaderModule(stage, spirv, entryPoint);
    }

    public override Framebuffer CreateFramebuffer(in ReadOnlySpan<Texture> colorTextures, Texture depthTexture)
    {
        return new D3D11Framebuffer(Device, colorTextures, depthTexture);
    }

    public override DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutDescription description)
    {
        return new D3D11DescriptorLayout(description);
    }

    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, in ReadOnlySpan<DescriptorSetDescription> descriptions)
    {
        return new D3D11DescriptorSet(descriptions.ToArray());
    }

    public override void UpdateDescriptorSet(DescriptorSet set, in ReadOnlySpan<DescriptorSetDescription> descriptions)
    {
        D3D11DescriptorSet d3dSet = (D3D11DescriptorSet) set;

        d3dSet.Descriptions = descriptions.ToArray();
    }
    
    public override IntPtr MapBuffer(Buffer buffer, MapMode mapMode)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        MappedSubresource subresource = Context.Map(d3dBuffer.Buffer, D3D11Utils.MapModeToD3D(mapMode));

        return subresource.DataPointer;
    }

    public override void UnmapBuffer(Buffer buffer)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        Context.Unmap(d3dBuffer.Buffer);
    }

    public override void ExecuteCommandList(CommandList list)
    { 
        Context.ExecuteCommandList(((D3D11CommandList) list).CommandList, false); 
    }

    public override void Dispose()
    {
        Context.Dispose();
        Device.Dispose();
    }
}