using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.Graphics.D3D12;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D12Device : Device
{
    public override bool IsDisposed { get; protected set; }

    public readonly ID3D12Device* Device;
    
    public override ShaderFormat ShaderFormat => ShaderFormat.Dxil;

    public D3D12Device(IDXGIAdapter1* adapter)
    {
        GrabsLog.Log("Creating D3D12 device.");
        fixed (ID3D12Device** device = &Device)
        {
            D3D12CreateDevice((IUnknown*) adapter, D3D_FEATURE_LEVEL_11_0, __uuidof<ID3D12Device>(), (void**) device)
                .Check("Create D3D12 device");
        }
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override CommandList CreateCommandList()
    {
        throw new NotImplementedException();
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] code, string entryPoint)
    {
        throw new NotImplementedException();
    }

    public override void ExecuteCommandList(CommandList cl)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        Device->Release();
    }
}