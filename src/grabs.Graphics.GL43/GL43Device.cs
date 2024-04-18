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

    public override unsafe Buffer CreateBuffer<T>(in BufferDescription description, in ReadOnlySpan<T> data)
    {
        fixed (void* pData = data)
            return new GL43Buffer(_gl, description, pData);
    }

    public override ShaderModule CreateShaderModule(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        return new GL43ShaderModule(_gl, stage, spirv, entryPoint);
    }

    public override Framebuffer CreateFramebuffer(in ReadOnlySpan<Texture> colorTextures, Texture depthTexture)
    {
        // As OpenGL doesn't support a user accessible swapchain framebuffer, we have to fake it.
        if (colorTextures[0] is GL43SwapchainTexture)
            return new GL43SwapchainFramebuffer();

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

                    if (desc.Framebuffer is GL43SwapchainFramebuffer)
                        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    else
                        throw new NotImplementedException();
                    
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