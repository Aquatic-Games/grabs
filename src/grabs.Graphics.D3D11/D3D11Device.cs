using System;
using System.Runtime.CompilerServices;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

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

        // TODO: While this SHOULD be fine for my purposes. I'd like to make this a customizable value, with the addition of Vulkan.
        Device.QueryInterface<IDXGIDevice1>().MaximumFrameLatency = 1;
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

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint,
        SpecializationConstant[] constants)
    {
        return new D3D11ShaderModule(stage, spirv, entryPoint, constants);
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

    public override unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData)
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

            MappedSubresource mResource = Context.Map(d3dBuffer.Buffer, Vortice.Direct3D11.MapMode.WriteDiscard);
            Unsafe.CopyBlock((byte*) mResource.DataPointer + offsetInBytes, pData, sizeInBytes);
            Context.Unmap(d3dBuffer.Buffer);
        }
        else
        {
            Context.UpdateSubresource(d3dBuffer.Buffer, 0,
                new Box((int) offsetInBytes, 0, 0, (int) (offsetInBytes + sizeInBytes), 1, 1), (nint) pData, 0, 0);
        }
    }

    public override unsafe void UpdateTexture(Texture texture, int x, int y, uint width, uint height, uint mipLevel, void* pData)
    {
        D3D11Texture d3dTexture = (D3D11Texture) texture;

        int pitch = (int) GraphicsUtils.CalculatePitch(d3dTexture.Format, width);

        Context.UpdateSubresource(d3dTexture.Texture, 0, new Box(x, y, 0, x + (int) width, y + (int) height, 1),
            (IntPtr) pData, pitch, 0);
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