using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.D3D11;

public sealed class D3D11Swapchain : Swapchain
{
    public readonly IDXGISwapChain SwapChain;

    public D3D11Swapchain(IDXGIFactory factory, ID3D11Device device, D3D11Surface surface, SwapchainDescription description)
    {
        SwapChainDescription desc = new SwapChainDescription()
        {
            Windowed = true,
            BufferDescription = new ModeDescription((int) description.Width, (int) description.Height),
            BufferCount = (int) description.BufferCount,
            OutputWindow = surface.Hwnd,
            SampleDescription = new SampleDescription(1, 0),
            BufferUsage = Usage.RenderTargetOutput,
            SwapEffect = SwapEffect.FlipDiscard,
            Flags = SwapChainFlags.AllowTearing | SwapChainFlags.AllowModeSwitch
        };

        SwapChain = factory.CreateSwapChain(device, desc);
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