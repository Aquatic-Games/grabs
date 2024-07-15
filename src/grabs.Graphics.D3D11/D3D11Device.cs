using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D_DRIVER_TYPE;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.D3D11;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.D3D11_CREATE_DEVICE_FLAG;
using static TerraFX.Interop.Windows.Windows;
using static grabs.Graphics.D3D11.D3DResult;
using static TerraFX.Interop.DirectX.D3D11_MAP;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11Device : Device
{
    private readonly IDXGIFactory1* _factory;
    private readonly D3D11Surface _surface;

    private D3D11Swapchain _swapchain;
    
    public readonly ID3D11Device* Device;
    public readonly ID3D11DeviceContext* Context;
    
    public D3D11Device(IDXGIFactory1* factory, D3D11Surface surface, IDXGIAdapter1* adapter)
    {
        _factory = factory;
        _surface = surface;

        D3D_FEATURE_LEVEL level = D3D_FEATURE_LEVEL_11_0;
        const D3D11_CREATE_DEVICE_FLAG flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
        
        fixed (ID3D11Device** device = &Device)
        fixed (ID3D11DeviceContext** context = &Context)
        {
            CheckResult(
                D3D11CreateDevice((IDXGIAdapter*) adapter, D3D_DRIVER_TYPE_UNKNOWN, HMODULE.NULL, (uint) flags, &level,
                    1, D3D11_SDK_VERSION, device, null, context), "Create device");
        }

        // TODO: While this SHOULD be fine for my purposes. I'd like to make this a customizable value, with the addition of Vulkan.
        IDXGIDevice1* dxgiDevice;
        CheckResult(Device->QueryInterface(__uuidof<IDXGIDevice1>(), (void**) &dxgiDevice));
        CheckResult(dxgiDevice->SetMaximumFrameLatency(1));
    }

    public override Swapchain CreateSwapchain(in SwapchainDescription description)
    {
        // TODO: Support multiple swapchains.
        _swapchain = new D3D11Swapchain((IDXGIFactory*) _factory, Device, Context, _surface, description);
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

    public override unsafe void UpdateTexture(Texture texture, int x, int y, uint width, uint height, uint mipLevel, void* pData)
    {
        D3D11Texture d3dTexture = (D3D11Texture) texture;

        uint pitch = GraphicsUtils.CalculatePitch(d3dTexture.Format, width);

        D3D11_BOX box = new D3D11_BOX(x, y, 0, x + (int) width, y + (int) height, 1);
        
        Context->UpdateSubresource(d3dTexture.Texture, 0, &box, pData, pitch, 0);
    }

    public override void UpdateDescriptorSet(DescriptorSet set, in ReadOnlySpan<DescriptorSetDescription> descriptions)
    {
        D3D11DescriptorSet d3dSet = (D3D11DescriptorSet) set;

        d3dSet.Descriptions = descriptions.ToArray();
    }
    
    public override nint MapBuffer(Buffer buffer, MapMode mapMode)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        D3D11_MAPPED_SUBRESOURCE subresource;
        Context->Map((ID3D11Resource*) d3dBuffer.Buffer, 0, D3D11Utils.MapModeToD3D(mapMode), 0, &subresource);

        return (nint) subresource.pData;
    }

    public override void UnmapBuffer(Buffer buffer)
    {
        D3D11Buffer d3dBuffer = (D3D11Buffer) buffer;
        Context->Unmap((ID3D11Resource*) d3dBuffer.Buffer, 0);
    }

    public override void ExecuteCommandList(CommandList list)
    { 
        Context->ExecuteCommandList(((D3D11CommandList) list).CommandList, false); 
    }

    public override void Dispose()
    {
        Context->Release();
        Device->Release();
    }
}