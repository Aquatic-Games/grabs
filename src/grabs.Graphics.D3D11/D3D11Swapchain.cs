using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXGI_SWAP_EFFECT;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Swapchain : Swapchain
{
    public override bool IsDisposed { get; protected set; }
    
    public override Format BufferFormat { get; }

    private readonly IDXGISwapChain* _swapchain;

    private D3D11Texture _swapchainTexture;

    public D3D11Swapchain(IDXGIFactory1* factory, ID3D11Device* device, ref readonly SwapchainInfo info)
    {
        BufferFormat = info.Format;
        
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
            BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,

            SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
        };

        GrabsLog.Log("Creating swapchain.");
        fixed (IDXGISwapChain** swapchain = &_swapchain)
            factory->CreateSwapChain((IUnknown*) device, &swapchainDesc, swapchain).Check("Create swapchain");

        GrabsLog.Log("Getting swapchain texture.");
        ID3D11Texture2D* swapchainTexture;
        _swapchain->GetBuffer(0, __uuidof<ID3D11Texture2D>(), (void**) &swapchainTexture)
            .Check("Get swapchain texture");

        _swapchainTexture = new D3D11Texture(device, swapchainTexture, info.Size);
    }
    
    public override Texture GetNextTexture()
    {
        return _swapchainTexture;
    }
    
    public override void Present()
    {
        _swapchain->Present(1, 0).Check("Present");
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        _swapchainTexture.Dispose();
        _swapchain->Release();
    }
}