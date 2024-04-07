using SharpGen.Runtime;
using Vortice.DXGI;

namespace grabs.D3D11;

public sealed class D3D11Instance : Instance
{
    public readonly IDXGIFactory1 Factory;

    public D3D11Instance()
    {
        Result result;

        if ((result = DXGI.CreateDXGIFactory1(out Factory!)).Failure)
            throw new Exception($"Failed to create DXGI factory. {result.Description}");
    }
    
    public override void Dispose()
    {
        Factory.Dispose();
    }
}