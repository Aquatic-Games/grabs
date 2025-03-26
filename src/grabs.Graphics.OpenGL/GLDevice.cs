using Silk.NET.OpenGL;

namespace grabs.Graphics.OpenGL;

internal sealed class GLDevice : Device
{
    private readonly GL _gl;

    public override Adapter Adapter => new Adapter(0, 0, _gl.GetStringS(StringName.Renderer), AdapterType.Dedicated, 0,
        new AdapterFeatures(), new AdapterLimits());

    public GLDevice(GL gl)
    {
        _gl = gl;
    }
    
    public override Swapchain CreateSwapchain(in SwapchainInfo info)
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
    
    public override unsafe Texture CreateTexture(in TextureInfo info, void* pData)
    {
        throw new NotImplementedException();
    }
    
    public override Pipeline CreatePipeline(in PipelineInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override void ExecuteCommandList(CommandList list)
    {
        throw new NotImplementedException();
    }
    
    public override unsafe void UpdateBuffer(Buffer buffer, uint offset, uint size, void* pData)
    {
        throw new NotImplementedException();
    }
    
    public override void WaitForIdle()
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}