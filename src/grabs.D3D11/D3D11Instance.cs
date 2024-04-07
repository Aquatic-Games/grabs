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
        {
            AdapterDescription1 desc = adapter.Description1;

            AdapterType type = AdapterType.Discrete;

            if ((desc.Flags & AdapterFlags.Software) != 0)
                type = AdapterType.Software;
                
            adapters.Add(new Adapter(i, desc.Description, (uint) desc.DedicatedVideoMemory, type));
        }

        return adapters.ToArray();
    }

    public override void Dispose()
    {
        Factory.Dispose();
    }
}