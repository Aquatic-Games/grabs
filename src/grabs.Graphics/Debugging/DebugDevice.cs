namespace grabs.Graphics.Debugging;

internal sealed class DebugDevice(Device device) : Device
{
    public override bool IsDisposed
    {
        get => device.IsDisposed;
        protected set => throw new NotImplementedException();
    }

    public override ShaderFormat ShaderFormat => device.ShaderFormat;

    public override Swapchain CreateSwapchain(in SwapchainInfo info)
        => new DebugSwapchain(device.CreateSwapchain(in info));

    public override CommandList CreateCommandList()
        => new DebugCommandList(device.CreateCommandList());

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] code, string entryPoint)
        => device.CreateShaderModule(stage, code, entryPoint);

    public override Pipeline CreateGraphicsPipeline(in GraphicsPipelineInfo info)
        => new DebugPipeline(device, in info);

    public override unsafe Buffer CreateBuffer(in BufferInfo info, void* pData)
    {
        throw new NotImplementedException();
    }

    public override void ExecuteCommandList(CommandList cl)
    {
        DebugCommandList debugCl = (DebugCommandList) cl;

        if (debugCl.IsBegun)
        {
            throw new ValidationException(
                "Cannot execute command list! The command list is currently active. You must call End() before it can be executed.");
        }

        if (!debugCl.HasIssuedCommands)
            throw new ValidationException("Cannot execute command list! No commands have been issued to it.");
        
        device.ExecuteCommandList(debugCl.CommandList);
    }
    
    public override void Dispose()
    {
        device.Dispose();
    }
}