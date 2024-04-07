using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.D3D11;

public class D3D11Swapchain : Swapchain
{
    public IDXGISwapChain SwapChain;

    public D3D11Swapchain(IDXGIFactory factory, ID3D11Device device, SwapchainDescription description)
    {
        
        factory.CreateSwapChain(device, )
    }
    
    public override void Present()
    {
        SwapChain.Present(1);
    }

    public override void Dispose()
    {
        SwapChain.Dispose();
    }
}