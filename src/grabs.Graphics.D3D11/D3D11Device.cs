using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D_DRIVER_TYPE;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.D3D11_CREATE_DEVICE_FLAG;
using static TerraFX.Interop.DirectX.D3D11;
using static TerraFX.Interop.DirectX.DirectX;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Device : Device
{
    public override bool IsDisposed { get; protected set; }

    public override ShaderFormat ShaderFormat => ShaderFormat.Dxbc;

    private readonly IDXGIFactory1* _factory;
    
    private readonly ID3D11Device* _device;
    private readonly ID3D11DeviceContext* _context;

    public D3D11Device(IDXGIAdapter1* adapter, IDXGIFactory1* factory, bool debug)
    {
        _factory = factory;

        uint flags = (uint) D3D11_CREATE_DEVICE_BGRA_SUPPORT;
        if (debug)
            flags |= (uint) D3D11_CREATE_DEVICE_DEBUG;

        D3D_FEATURE_LEVEL featureLevel = D3D_FEATURE_LEVEL_11_1;
        
        GrabsLog.Log("Creating D3D11 device.");
        fixed (ID3D11Device** device = &_device)
        fixed (ID3D11DeviceContext** context = &_context)
        {
            D3D11CreateDevice((IDXGIAdapter*) adapter, D3D_DRIVER_TYPE_UNKNOWN, HMODULE.NULL, flags, &featureLevel, 1,
                D3D11_SDK_VERSION, device, null, context).Check("Create device");
        }
    }
    
    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new D3D11Swapchain(_factory, _device, in info);
    }
    
    public override CommandList CreateCommandList()
    {
        return new D3D11CommandList(_device);
    }
    
    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] code, string entryPoint)
    {
        return new D3D11ShaderModule(code);
    }
    
    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
    {
        return new D3D11Pipeline(_device, in info);
    }
    
    public override void ExecuteCommandList(CommandList cl)
    {
        _context->ExecuteCommandList(((D3D11CommandList) cl).CommandList, false);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        _context->Release();
        _device->Release();
    }
}