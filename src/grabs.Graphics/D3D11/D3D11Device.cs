using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Device : Device
{
    public readonly ID3D11Device Device;

    public readonly ID3D11DeviceContext Context;

    public D3D11Device(IDXGIAdapter1 adapter, bool debug)
    {
        DeviceCreationFlags creationFlags = DeviceCreationFlags.BgraSupport;
        if (debug)
            creationFlags |= DeviceCreationFlags.Debug;

        Vortice.Direct3D11.D3D11.D3D11CreateDevice(adapter, DriverType.Unknown, creationFlags,
            [FeatureLevel.Level_11_1], out Device, out Context).CheckError();
    }
    
    public override Swapchain CreateSwapchain(Surface surface, in SwapchainInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override CommandList CreateCommandList()
    {
        throw new NotImplementedException();
    }
    
    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        throw new NotImplementedException();
    }
    
    public override unsafe Buffer CreateBuffer(in BufferInfo info, void* pData)
    {
        throw new NotImplementedException();
    }
    
    public override Pipeline CreatePipeline(in PipelineInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override void ExecuteCommandList(CommandList list)
    {
        throw new NotImplementedException();
    }
    
    public override void WaitForIdle()
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        Device.Dispose();
    }
}