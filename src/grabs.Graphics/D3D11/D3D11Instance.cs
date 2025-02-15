using grabs.Core;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Instance : Instance
{
    private readonly bool _debug;
    
    public readonly IDXGIFactory1 Factory;
    
    public override Backend Backend => Backend.D3D11;

    public D3D11Instance(ref readonly InstanceInfo info)
    {
        _debug = info.Debug;
        
        GrabsLog.Log("Creating DXGI factory.");
        Factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = new List<Adapter>();
        
        for (uint i = 0; Factory.EnumAdapters1(i, out IDXGIAdapter1 adapter).Success; i++)
        {
            AdapterDescription1 desc = adapter.Description1;
            
            string name = desc.Description;

            AdapterType type = AdapterType.Dedicated;
            if (desc.Flags.HasFlag(AdapterFlags.Software))
                type = AdapterType.Software;

            ulong memory = desc.DedicatedVideoMemory;
            
            adapters.Add(new Adapter(adapter.NativePointer, i, name, type, memory));
        }

        return adapters.ToArray();
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        IDXGIAdapter1 dxgiAdapter;
        
        if (adapter is { } givenAdapter)
            dxgiAdapter = new IDXGIAdapter1(givenAdapter.Handle);
        else
        {
            Adapter[] adapters = EnumerateAdapters();
            dxgiAdapter = new IDXGIAdapter1(adapters[0].Handle);
        }

        return new D3D11Device(Factory, dxgiAdapter, _debug);
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new D3D11Surface(in info);
    }
    
    public override void Dispose()
    {
        Factory.Dispose();
    }
}