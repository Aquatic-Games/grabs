using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.DirectX.DXGI_SWAP_CHAIN_FLAG;
using static TerraFX.Interop.DirectX.DXGI_SWAP_EFFECT;
using static TerraFX.Interop.Windows.Windows;
using static grabs.Graphics.D3D11.D3DResult;
using static TerraFX.Interop.DirectX.DXGI_FORMAT;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11Swapchain : Swapchain
{
    private PresentMode _presentMode;
    private uint _swapInterval;

    private ID3D11DeviceContext* _context;
    
    internal List<D3D11CommandList> CommandLists;
    
    public readonly IDXGISwapChain* SwapChain;

    public override PresentMode PresentMode
    {
        get => _presentMode;
        set
        {
            (_presentMode, _swapInterval) = value switch
            {
                PresentMode.Immediate => (PresentMode.Immediate, 0u),
                PresentMode.VerticalSync => (PresentMode.VerticalSync, 1u),
                // Far as I can see, DXGI 1.1 does not support AS, so it becomes VS instead.
                PresentMode.AdaptiveSync => (PresentMode.VerticalSync, 1u),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public D3D11Swapchain(IDXGIFactory* factory, ID3D11Device* device, ID3D11DeviceContext* context, D3D11Surface surface, SwapchainDescription description)
    {
        _context = context;

        CommandLists = new List<D3D11CommandList>();
        
        DXGI_SWAP_CHAIN_DESC desc = new()
        {
            Windowed = true,
            BufferDesc = new DXGI_MODE_DESC()
            {
                Width = description.Width,
                Height = description.Height,
                Format = D3D11Utils.FormatToD3D(description.Format)
            },
            BufferCount = description.BufferCount,
            OutputWindow = (HWND) surface.Hwnd,
            SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
            BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            Flags = (uint) (DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING | DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH)
        };
        
        PresentMode = description.PresentMode;

        fixed(IDXGISwapChain** swapchain = &SwapChain)
            CheckResult(factory->CreateSwapChain((IUnknown*) device, &desc, swapchain), "Create swapchain");
    }

    public override Texture GetSwapchainTexture()
    {
        ID3D11Texture2D* texture;
        CheckResult(SwapChain->GetBuffer(0, __uuidof<ID3D11Texture2D>(), (void**) &texture), "Get swapchain buffer");
        
        return new D3D11Texture((ID3D11Resource*) texture, null);
    }

    public override void Resize(uint width, uint height)
    {
        _context->Flush();
        
        foreach (D3D11CommandList list in CommandLists)
        {
            if (list.CommandList != null)
                list.CommandList->Release();
            list.CommandList = null;

            list.Context->ClearState();
            ID3D11CommandList* cl;
            list.Context->FinishCommandList(false, &cl);
            cl->Release();
        }
        
        _context->ClearState();
        _context->Flush();

        CheckResult(
            SwapChain->ResizeBuffers(0, width, height, DXGI_FORMAT_UNKNOWN,
                (uint) (DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING | DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH)),
            "Resize swapchain buffers");
    }

    public override void Present()
    {
        CheckResult(SwapChain->Present(_swapInterval, 0), "Present");
    }

    public override void Dispose()
    {
        SwapChain->Release();
    }
}