using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.DXGI_ADAPTER_FLAG;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Instance : Instance
{
    public override bool IsDisposed { get; protected set; }

    private readonly bool _debug;
    private readonly IDXGIFactory1* _factory;

    public override string BackendName => D3D11Backend.Name;

    public D3D11Instance(ref readonly InstanceInfo info)
    {
        _debug = info.Debug;
        
        if (!OperatingSystem.IsWindows())
            ResolveLibrary += OnResolveLibrary; 
        
        GrabsLog.Log("Creating DXGI 1.1 factory.");
        fixed (IDXGIFactory1** factory = &_factory)
            CreateDXGIFactory1(__uuidof<IDXGIFactory1>(), (void**) factory).Check("Create DXGI factory");
    }

    private static IntPtr OnResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        libraryName = libraryName switch
        {
            "dxgi" => "dxvk_dxgi",
            "d3d11" => "dxvk_d3d11",
            _ => libraryName
        };

        return NativeLibrary.Load(libraryName, assembly, searchPath);
    }

    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = [];

        IDXGIAdapter1* dxgiAdapter;
        for (uint i = 0; _factory->EnumAdapters1(i, &dxgiAdapter).SUCCEEDED; i++)
        {
            DXGI_ADAPTER_DESC1 desc;
            dxgiAdapter->GetDesc1(&desc).Check("Get adapter desc");

            string name = new string(desc.Description);
            
            AdapterType type = (desc.Flags & (uint) DXGI_ADAPTER_FLAG_SOFTWARE) == (uint) DXGI_ADAPTER_FLAG_SOFTWARE
                ? AdapterType.Software
                : AdapterType.Dedicated;

            ulong dedicatedMemory = desc.DedicatedVideoMemory;
            
            adapters.Add(new Adapter((nint) dxgiAdapter, i, name, type, dedicatedMemory));
        }
        
        return adapters.ToArray();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new D3D11Surface(in info);
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        IDXGIAdapter1* dxgiAdapter;

        if (adapter is Adapter adp)
            dxgiAdapter = (IDXGIAdapter1*) adp.Handle;
        else
            _factory->EnumAdapters1(0, &dxgiAdapter).Check("Enumerate adapter");

        return new D3D11Device(dxgiAdapter, _factory, _debug);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        _factory->Release();
    }
}