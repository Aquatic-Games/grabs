using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.DXGI;

namespace grabs.Graphics.D3D12;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D12Instance : Instance
{
    public override bool IsDisposed { get; protected set; }

    private readonly IDXGIFactory2* _factory;

    public override string BackendName => D3D12Backend.Name;

    public D3D12Instance(ref readonly InstanceInfo info)
    {
        uint flags = 0;
        GrabsLog.Log("Creating DXGI factory.");
        fixed (IDXGIFactory2** factory = &_factory)
            CreateDXGIFactory2(flags, Windows.__uuidof<IDXGIFactory2>(), (void**) factory).Check("Create DXGI factory");
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = [];

        IDXGIAdapter1* adapter;
        for (uint i = 0; _factory->EnumAdapters1(i, &adapter).SUCCEEDED; i++)
        {
            DXGI_ADAPTER_DESC1 desc;
            adapter->GetDesc1(&desc).Check("Get adapter desc");

            string name = new string(desc.Description);

            adapters.Add(new Adapter((nint) adapter, i, name, AdapterType.Dedicated, desc.DedicatedVideoMemory));
        }

        return adapters.ToArray();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new D3D12Surface(in info);
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        IDXGIAdapter1* dAdapter;
        if (adapter is Adapter adp)
            dAdapter = (IDXGIAdapter1*) adp.Handle;
        else
            _factory->EnumAdapters1(0, &dAdapter).Check("Enumerate adapter");

        return new D3D12Device(dAdapter);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        _factory->Release();
    }
}