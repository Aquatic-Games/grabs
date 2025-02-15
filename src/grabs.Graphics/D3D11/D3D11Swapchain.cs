using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Swapchain : Swapchain
{
    public readonly IDXGISwapChain Swapchain;
    
    public D3D11Swapchain(IDXGIFactory factory, ID3D11Device device, D3D11Surface surface, ref readonly SwapchainInfo info)
    {
        SwapChainDescription swapchainDesc = new SwapChainDescription()
        {
            OutputWindow = surface.Hwnd,
            Windowed = true,

            BufferCount = info.NumBuffers,
            BufferDescription = new ModeDescription(info.Size.Width, info.Size.Height, info.Format.ToD3D()),
            BufferUsage = Usage.RenderTargetOutput,
            
            SampleDescription = new SampleDescription(1, 0),
            SwapEffect = SwapEffect.FlipDiscard
        };

        Swapchain = factory.CreateSwapChain(device, swapchainDesc);
    }
    
    public override Texture GetNextTexture()
    {
        throw new NotImplementedException();
    }
    
    public override void Present()
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        Swapchain.Dispose();
    }
}