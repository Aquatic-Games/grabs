using System;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Swapchain : Swapchain
{
    private PresentMode _presentMode;
    private int _swapInterval;
    
    public readonly IDXGISwapChain SwapChain;
    
    public readonly ID3D11Texture2D SwapChainTexture;

    public override PresentMode PresentMode
    {
        get => _presentMode;
        set
        {
            (_presentMode, _swapInterval) = value switch
            {
                PresentMode.Immediate => (PresentMode.Immediate, 0),
                PresentMode.VerticalSync => (PresentMode.VerticalSync, 1),
                // Far as I can see, DXGI 1.1 does not support AS, so it becomes VS instead.
                PresentMode.AdaptiveSync => (PresentMode.VerticalSync, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public D3D11Swapchain(IDXGIFactory factory, ID3D11Device device, D3D11Surface surface, SwapchainDescription description)
    {
        SwapChainDescription desc = new SwapChainDescription()
        {
            Windowed = true,
            BufferDescription = new ModeDescription((int) description.Width, (int) description.Height, D3D11Utils.FormatToD3D(description.Format)),
            BufferCount = (int) description.BufferCount,
            OutputWindow = surface.Hwnd,
            SampleDescription = new SampleDescription(1, 0),
            BufferUsage = Usage.RenderTargetOutput,
            SwapEffect = SwapEffect.FlipDiscard,
            Flags = SwapChainFlags.AllowTearing | SwapChainFlags.AllowModeSwitch
        };
        
        PresentMode = description.PresentMode;

        SwapChain = factory.CreateSwapChain(device, desc);

        SwapChainTexture = SwapChain.GetBuffer<ID3D11Texture2D>(0);
    }

    public override Texture GetSwapchainTexture()
    {
        return new D3D11Texture(SwapChainTexture, null);
    }

    public override void Present()
    {
        SwapChain.Present(_swapInterval);
    }

    public override void Dispose()
    {
        SwapChainTexture.Dispose();
        SwapChain.Dispose();
    }
}