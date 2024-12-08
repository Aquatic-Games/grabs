using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXGI_SWAP_CHAIN_FLAG;
using static TerraFX.Interop.DirectX.DXGI_SWAP_EFFECT;
using static TerraFX.Interop.DirectX.DXGI;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Swapchain : Swapchain
{
    public IDXGISwapChain* Swapchain;

    public D3D11Swapchain(IDXGIFactory1* factory, ID3D11Device* device, D3D11Surface surface, ref readonly SwapchainDescription description)
    {
        DXGI_SWAP_CHAIN_DESC desc = new()
        {
            OutputWindow = new HWND((void*) surface.Hwnd),
            Windowed = true,
            
            BufferCount = description.NumBuffers,
            BufferDesc = new DXGI_MODE_DESC
            {
                Width = description.Size.Width,
                Height = description.Size.Height,
                Format = description.Format.ToD3D()
            },
            BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            
            SampleDesc = new DXGI_SAMPLE_DESC(1, 0),
            SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            
            Flags = (uint) (DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING | DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH)
        };
        
        fixed (IDXGISwapChain** swapchain = &Swapchain)
            D3D11Result.CheckResult(factory->CreateSwapChain((IUnknown*) device, &desc, swapchain), "Create swapchain");
    }

    public override void Present()
    {
        Swapchain->Present(1, DXGI_PRESENT_ALLOW_TEARING);
    }

    public override void Dispose()
    {
        Swapchain->Release();
    }
}