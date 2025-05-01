using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXGI_SWAP_EFFECT;
using static TerraFX.Interop.DirectX.DXGI;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Swapchain : Swapchain
{
    public override bool IsDisposed { get; protected set; }
    
    public override Format BufferFormat { get; }

    private readonly IDXGISwapChain* _swapchain;

    public D3D11Swapchain(IDXGIFactory1* factory, ID3D11Device* device, ref readonly SwapchainInfo info)
    {
        DXGI_SWAP_CHAIN_DESC swapchainDesc = new()
        {
            OutputWindow = (HWND) ((D3D11Surface) info.Surface).Hwnd,
            Windowed = true,

            BufferDesc = new DXGI_MODE_DESC
            {
                Width = info.Size.Width,
                Height = info.Size.Height,
                Format = info.Format.ToD3D()
            },
            BufferCount = info.NumBuffers,
            BufferUsage = DXGI_USAGE_BACK_BUFFER,

            SwapEffect = DXGI_SWAP_EFFECT_DISCARD,
            SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
        };

        GrabsLog.Log("Creating swapchain.");
        fixed (IDXGISwapChain** swapchain = &_swapchain)
            factory->CreateSwapChain((IUnknown*) device, &swapchainDesc, swapchain).Check("Create swapchain");
    }
    
    public override Texture GetNextTexture()
    {
        return null;
    }
    
    public override void Present()
    {
        _swapchain->Present(1, 0);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        _swapchain->Release();
    }
}