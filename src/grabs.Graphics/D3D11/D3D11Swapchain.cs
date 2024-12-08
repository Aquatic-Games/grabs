using TerraFX.Interop.DirectX;

namespace grabs.Graphics.D3D11;

internal sealed unsafe class D3D11Swapchain : Swapchain
{
    public IDXGISwapChain* Swapchain;

    public D3D11Swapchain(IDXGIFactory1* factory)
    {
        
    }
    
    public override void Dispose()
    {
        
    }
}