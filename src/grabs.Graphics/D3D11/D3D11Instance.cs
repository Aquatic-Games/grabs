using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Instance : Instance
{
    public readonly IDXGIFactory1 Factory;
    
    public override Backend Backend => Backend.D3D11;

    public D3D11Instance(ref readonly InstanceInfo info)
    {
        Factory = DXGI.CreateDXGIFactory1<IDXGIFactory1>();
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        throw new NotImplementedException();
    }
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}