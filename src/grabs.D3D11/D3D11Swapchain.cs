﻿using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.D3D11;

public sealed class D3D11Swapchain : Swapchain
{
    public readonly IDXGISwapChain SwapChain;
    
    public readonly ID3D11Texture2D SwapChainTexture;
    public readonly ID3D11RenderTargetView SwapChainTarget;

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

        SwapChainTexture = SwapChain.GetBuffer<ID3D11Texture2D>(0);
        SwapChainTarget = device.CreateRenderTargetView(SwapChainTexture);
    }
    
    public override void Present()
    {
        SwapChain.Present(1);
    }

    public override void Dispose()
    {
        SwapChainTarget.Dispose();
        SwapChainTexture.Dispose();
        SwapChain.Dispose();
    }
}