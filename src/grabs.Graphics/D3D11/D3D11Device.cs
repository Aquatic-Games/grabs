using TerraFX.Interop.DirectX;

namespace grabs.Graphics.D3D11;

internal sealed unsafe class D3D11Device : Device
{
    public D3D11Device(IDXGIFactory1* factory, IDXGIAdapter1* adapter)
    {
        
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}