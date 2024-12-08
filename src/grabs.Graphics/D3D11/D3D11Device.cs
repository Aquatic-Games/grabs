using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static grabs.Graphics.D3D11.D3D11Result;
using static TerraFX.Interop.DirectX.D3D_DRIVER_TYPE;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.D3D11_CREATE_DEVICE_FLAG;
using static TerraFX.Interop.DirectX.D3D11;
using static TerraFX.Interop.DirectX.DirectX;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Device : Device
{
    private readonly IDXGIFactory1* _factory;
    
    public readonly ID3D11Device* Device;
    
    public readonly ID3D11DeviceContext* Context;
    
    public D3D11Device(bool debug, IDXGIFactory1* factory, IDXGIAdapter1* adapter)
    {
        _factory = factory;
        
        D3D11_CREATE_DEVICE_FLAG flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
        if (debug)
            flags |= D3D11_CREATE_DEVICE_DEBUG;

        D3D_FEATURE_LEVEL level = D3D_FEATURE_LEVEL_11_1;
        
        fixed (ID3D11Device** device = &Device)
        fixed (ID3D11DeviceContext** context = &Context)
        {
            CheckResult(
                D3D11CreateDevice((IDXGIAdapter*) adapter, D3D_DRIVER_TYPE_UNKNOWN, HMODULE.NULL, (uint) flags, &level,
                    1, D3D11_SDK_VERSION, device, null, context), "Create device");
        }
    }

    public override Swapchain CreateSwapchain(Surface surface, ref readonly SwapchainDescription description)
    {
        return new D3D11Swapchain(_factory, Device, (D3D11Surface) surface, in description);
    }

    public override void Dispose()
    {
        Context->Release();
        Device->Release();
    }
}