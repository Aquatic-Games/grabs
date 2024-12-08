using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.Windows;
using static grabs.Graphics.D3D11.D3D11Result;
using static TerraFX.Interop.DirectX.DXGI_ADAPTER_FLAG;
using static TerraFX.Interop.DirectX.DXGI;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Instance : Instance
{
    public readonly IDXGIFactory1* Factory;
    
    public override bool IsDisposed { get; protected set; }
    
    public override Backend Backend => Backend.D3D11;

    public D3D11Instance(bool debug)
    {
        fixed (IDXGIFactory1** factory = &Factory)
            CheckResult(CreateDXGIFactory1(__uuidof<IDXGIFactory1>(), (void**) factory), "Create DXGI factory");
    }
    
    
    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = new List<Adapter>();

        IDXGIAdapter1* dxgiAdapter;
        for (uint i = 0; Factory->EnumAdapters1(i, &dxgiAdapter) != DXGI_ERROR_NOT_FOUND; i++)
        {
            DXGI_ADAPTER_DESC1 desc;
            dxgiAdapter->GetDesc1(&desc);

            string name = new string((char*) &desc.Description);
            ulong dedicatedMemory = desc.DedicatedVideoMemory;
            AdapterType type = (desc.Flags & (uint) DXGI_ADAPTER_FLAG_SOFTWARE) == (uint) DXGI_ADAPTER_FLAG_SOFTWARE
                ? AdapterType.Software
                : AdapterType.Dedicated;
            
            adapters.Add(new Adapter(i, name, dedicatedMemory, type));
        }

        return adapters.ToArray();
    }

    public override void Dispose()
    {
        Factory->Release();
    }
}