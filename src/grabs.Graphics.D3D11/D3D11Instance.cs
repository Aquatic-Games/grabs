﻿using System;
using System.Collections.Generic;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.Graphics.D3D11;

public sealed unsafe class D3D11Instance : Instance
{
    public readonly IDXGIFactory1* Factory;

    public override GraphicsApi Api => GraphicsApi.D3D11;

    public D3D11Instance()
    {
        Result result;

        fixed (IDXGIFactory1** factory = &Factory)
        {
            if ((result = CreateDXGIFactory1(__uuidof<IDXGIFactory1>(), (void**) factory)))
                throw new Exception($"Failed to create DXGI factory. {result.Description}");
        }
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        Factory.EnumAdapters1((int) (adapter?.Index ?? 0), out IDXGIAdapter1 dxgiAdapter);

        return new D3D11Device(Factory, (D3D11Surface) surface, dxgiAdapter);
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
                
            adapters.Add(new Adapter(i, desc.Description, (ulong) (long) desc.DedicatedVideoMemory, type));
        }

        return adapters.ToArray();
    }

    public override void Dispose()
    {
        Factory.Dispose();
    }
}