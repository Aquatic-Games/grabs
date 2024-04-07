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

    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = new List<Adapter>();
        for (uint i = 0; Factory.EnumAdapters1((int) i, out IDXGIAdapter1 adapter).Success; i++)
            adapters.Add(new Adapter(i, adapter.Description1.Description));

        return adapters.ToArray();
    }

    public override void Dispose()
    {
        Factory.Dispose();
    }
}