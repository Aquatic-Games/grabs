using System;
using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Device : Device
{
    private readonly IDXGIFactory _factory;
    
    public readonly ID3D11Device Device;
    public readonly ID3D11DeviceContext Context;
    
    public D3D11Device(IDXGIFactory1 factory, IDXGIAdapter1 adapter)
    {
        _factory = factory;
        
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

    public override Swapchain CreateSwapchain(Surface surface, in SwapchainDescription description)
    {
        return new D3D11Swapchain(_factory, Device, (D3D11Surface) surface, description);
    }

    public override CommandList CreateCommandList()
    {
        return new D3D11CommandList(Device);
    }

    public override unsafe Buffer CreateBuffer<T>(in BufferDescription description, in ReadOnlySpan<T> data)
    {
        fixed (void* pData = data)
            return new D3D11Buffer(Device, description, pData);
    }

    public override ShaderModule CreateShaderModule(byte[] spirv, string entryPoint)
    {
        throw new NotImplementedException();
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