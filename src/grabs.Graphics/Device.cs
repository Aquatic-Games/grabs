namespace grabs.Graphics;

public abstract class Device : IDisposable
{
    public abstract Swapchain CreateSwapchain(Surface surface, in SwapchainInfo info);

    public abstract CommandList CreateCommandList();

    public abstract ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint);

    public abstract Pipeline CreatePipeline(in PipelineInfo info);

    public abstract void ExecuteCommandList(CommandList list);

    public abstract void WaitForIdle();
    
    public abstract void Dispose();
}