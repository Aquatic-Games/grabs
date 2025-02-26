using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Swapchain : Swapchain
{
    private readonly D3D11Texture _swapchainTexture;
    
    public readonly IDXGISwapChain Swapchain;
    
    public override Format SwapchainFormat { get; }
    
    public D3D11Swapchain(IDXGIFactory factory, ID3D11Device device, ref readonly SwapchainInfo info)
    {
        SwapchainFormat = info.Format;

        D3D11Surface surface = (D3D11Surface) info.Surface;
        
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

        ID3D11Texture2D texture = Swapchain.GetBuffer<ID3D11Texture2D>(0);
        ID3D11RenderTargetView target = device.CreateRenderTargetView(texture);

        _swapchainTexture = new D3D11Texture(texture, target, info.Size);
    }

    public override Texture GetNextTexture()
        => _swapchainTexture;
    
    public override void Present()
    {
        Swapchain.Present(1);
    }
    
    public override void Dispose()
    {
        Swapchain.Dispose();
    }
}