using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public class GL43Device : Device
{
    private readonly GL _gl;
    
    public GL43Device(GL gl)
    {
        _gl = gl;
    }
    
    public override Swapchain CreateSwapchain(Surface surface, in SwapchainDescription description)
    {
        return new GL43Swapchain((GL43Surface) surface, description);
    }

    public override CommandList CreateCommandList()
    {
        throw new NotImplementedException();
    }

    public override Buffer CreateBuffer<T>(in BufferDescription description, in ReadOnlySpan<T> data)
    {
        throw new NotImplementedException();
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        throw new NotImplementedException();
    }

    public override void ExecuteCommandList(CommandList list)
    {
        throw new NotImplementedException();
    }

    public override void Dispose() { }
}