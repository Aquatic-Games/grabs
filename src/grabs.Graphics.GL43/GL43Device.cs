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
        return new GL43CommandList();
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
        GL43CommandList cl = (GL43CommandList) list;

        foreach (CommandListAction action in cl.Actions)
        {
            switch (action.Type)
            {
                case CommandListActionType.BeginRenderPass:
                    RenderPassDescription desc = action.BeginRenderPass.RenderPassDescription;
                    
                    _gl.ClearColor(desc.ClearColor.X, desc.ClearColor.Y, desc.ClearColor.Z, desc.ClearColor.W);
                    _gl.Clear(ClearBufferMask.ColorBufferBit);
                    break;
                case CommandListActionType.EndRenderPass:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Dispose() { }
}