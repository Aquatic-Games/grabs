using System;
using System.Collections.Generic;
using System.Drawing;
using SharpGen.Runtime;
using Vortice.Direct3D11;
using Vortice.Direct3D11.Debug;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Swapchain : Swapchain
{
    private PresentMode _presentMode;
    private int _swapInterval;

    private ID3D11DeviceContext _context;
    
    internal List<D3D11CommandList> CommandLists;
    
    public readonly IDXGISwapChain SwapChain;

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

    public D3D11Swapchain(IDXGIFactory factory, ID3D11Device device, ID3D11DeviceContext context, D3D11Surface surface, SwapchainDescription description)
    {
        _context = context;

        CommandLists = new List<D3D11CommandList>();
        
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
    }

    public override Texture GetSwapchainTexture()
    {
        return new D3D11Texture(SwapChain.GetBuffer<ID3D11Texture2D>(0), null);
    }

    public override void Resize(Size size)
    {
        _context.Flush();
        
        foreach (D3D11CommandList list in CommandLists)
        {
            list.CommandList?.Dispose();
        }
        
        _context.ClearState();
        _context.Flush();
        
        SwapChain.ResizeBuffers(0, size.Width, size.Height, Vortice.DXGI.Format.Unknown,
            SwapChainFlags.AllowTearing | SwapChainFlags.AllowModeSwitch).CheckError();
    }

    public override void Present()
    {
        SwapChain.Present(_swapInterval);
    }

    public override void Dispose()
    {
        SwapChain.Dispose();
    }
}