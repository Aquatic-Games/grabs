using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.Windows;
using static grabs.Graphics.D3D11.D3DResult;
using static TerraFX.Interop.DirectX.DXGI_ADAPTER_FLAG;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11Instance : Instance
{
    public readonly IDXGIFactory1* Factory;

    public override GraphicsApi Api => GraphicsApi.D3D11;

    public D3D11Instance()
    {
        fixed (IDXGIFactory1** factory = &Factory)
            CheckResult(CreateDXGIFactory1(__uuidof<IDXGIFactory1>(), (void**) factory), "Create DXGI factory");
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        IDXGIAdapter1* dxgiAdapter;
        Factory->EnumAdapters1(adapter?.Index ?? 0, &dxgiAdapter);

        return new D3D11Device(Factory, (D3D11Surface) surface, dxgiAdapter);
    }

    public override Adapter[] EnumerateAdapters()
    {
        List<Adapter> adapters = new List<Adapter>();
        IDXGIAdapter1* adapter;
        for (uint i = 0; Factory->EnumAdapters1(i, &adapter).SUCCEEDED; i++)
        {
            DXGI_ADAPTER_DESC1 desc;
            adapter->GetDesc1(&desc);

            AdapterType type = AdapterType.Discrete;

            if ((desc.Flags & (uint) DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                type = AdapterType.Software;
                
            adapters.Add(new Adapter(i, new string(desc.Description), (ulong) (long) desc.DedicatedVideoMemory, type));
        }

        return adapters.ToArray();
    }

    public override void Dispose()
    {
        Factory->Release();
    }
}