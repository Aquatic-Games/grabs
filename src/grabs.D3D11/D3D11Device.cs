using SharpGen.Runtime;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.D3D11;

public sealed class D3D11Device : Device
{
    public ID3D11Device Device;
    public ID3D11DeviceContext Context;
    
    public D3D11Device(IDXGIFactory1 factory, IDXGIAdapter1 adapter)
    {
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

    public override void Dispose()
    {
        Context.Dispose();
        Device.Dispose();
    }
}