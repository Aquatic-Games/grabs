using System.Diagnostics;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Device : Device
{
    private readonly IDXGIFactory _factory;
    
    public readonly ID3D11Device Device;

    public readonly ID3D11DeviceContext Context;
    
    public override Adapter Adapter { get; }

    public D3D11Device(IDXGIFactory factory, in Adapter adapter, bool debug)
    {
        _factory = factory;
        
        DeviceCreationFlags creationFlags = DeviceCreationFlags.BgraSupport;
        if (debug)
            creationFlags |= DeviceCreationFlags.Debug;

        Debug.Assert(adapter.Handle != 0);
        IDXGIAdapter1 adapter1 = new IDXGIAdapter1(adapter.Handle);

        Vortice.Direct3D11.D3D11.D3D11CreateDevice(adapter1, DriverType.Unknown, creationFlags,
            [FeatureLevel.Level_11_1], out Device, out Context).CheckError();
    }

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
    {
        return new D3D11Swapchain(_factory, Device, in info);
    }
    
    public override CommandList CreateCommandList()
    {
        return new D3D11CommandList(Device);
    }
    
    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        return new D3D11ShaderModule(stage, spirv, entryPoint);
    }
    
    public override unsafe Buffer CreateBuffer(in BufferInfo info, void* pData)
    {
        return new D3D11Buffer(Device, Context, in info, pData);
    }
    
    public override Pipeline CreatePipeline(in PipelineInfo info)
    {
        return new D3D11Pipeline(Device, in info);
    }
    
    public override void ExecuteCommandList(CommandList list)
    {
        D3D11CommandList d3dList = (D3D11CommandList) list;
        
        Debug.Assert(d3dList.CommandList != null, "Command List was null, have you called End()?");
        
        Context.ExecuteCommandList(d3dList.CommandList, false);
    }
    
    public override void WaitForIdle()
    {
        Context.Flush();
    }
    
    public override void Dispose()
    {
        Context.Dispose();
        Device.Dispose();
    }
}