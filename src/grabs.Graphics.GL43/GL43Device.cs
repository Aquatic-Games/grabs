using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public class GL43Device : Device
{
    private readonly GL _gl;

    private GL43Swapchain _swapchain;
    
    public GL43Device(GL gl)
    {
        _gl = gl;
    }
    
    public override Swapchain CreateSwapchain(Surface surface, in SwapchainDescription description)
    {
        // Unfortunately we have to store a reference to the swapchain, so we can get its size, which are used in the
        // viewport and scissor commands.
        _swapchain = new GL43Swapchain((GL43Surface) surface, description);
        return _swapchain;
    }

    public override CommandList CreateCommandList()
    {
        return new GL43CommandList();
    }

    public override Pipeline CreatePipeline(in PipelineDescription description)
    {
        return new GL43Pipeline(_gl, description);
    }

    public override unsafe Buffer CreateBuffer(in BufferDescription description, void* pData)
    {
        return new GL43Buffer(_gl, description, pData);
    }

    public override unsafe Texture CreateTexture(in TextureDescription description, void* pData)
    {
        return new GL43Texture(_gl, description, pData);
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

    public override unsafe void ExecuteCommandList(CommandList list)
    {
        GL43CommandList cl = (GL43CommandList) list;

        foreach (CommandListAction action in cl.Actions)
        {
            switch (action.Type)
            {
                case CommandListActionType.BeginRenderPass:
                {
                    RenderPassDescription desc = action.RenderPassDescription;

                    if (desc.Framebuffer is GL43SwapchainFramebuffer)
                        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    else
                        throw new NotImplementedException();

                    switch (desc.ColorLoadOp)
                    {
                        case LoadOp.Clear:
                            _gl.ClearColor(desc.ClearColor.X, desc.ClearColor.Y, desc.ClearColor.Z, desc.ClearColor.W);
                            _gl.Clear(ClearBufferMask.ColorBufferBit);
                            break;
                        
                        case LoadOp.Load:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    break;
                }
                
                case CommandListActionType.EndRenderPass:
                    break;

                case CommandListActionType.GenerateMipmaps:
                {
                    GL43Texture texture = (GL43Texture) action.Texture;
                    _gl.BindTexture(texture.Target, texture.Texture);
                    _gl.GenerateMipmap(texture.Target);
                    
                    break;
                }

                case CommandListActionType.UpdateBuffer:
                {
                    GL43Buffer buffer = (GL43Buffer) action.Buffer;
                    GCHandle handle = GCHandle.Alloc(action.MiscObject, GCHandleType.Pinned);
                    
                    _gl.BindBuffer(BufferTargetARB.UniformBuffer, buffer.Buffer);
                    _gl.BufferSubData(BufferTargetARB.UniformBuffer, (nint) action.Offset, (nuint) action.Stride,
                        (void*) handle.AddrOfPinnedObject());
                    
                    handle.Free();

                    break;
                }

                case CommandListActionType.SetViewport:
                {
                    Viewport viewport = action.Viewport;
                    
                    _gl.Viewport(viewport.X, (int) _swapchain.Height - viewport.Y - (int) viewport.Height, viewport.Width, viewport.Height);
                    break;
                }

                case CommandListActionType.SetPipeline:
                {
                    GL43Pipeline pipeline = (GL43Pipeline) action.Pipeline;
                    
                    _gl.BindVertexArray(pipeline.Vao);
                    _gl.UseProgram(pipeline.ShaderProgram);
                    break;
                }
                
                case CommandListActionType.SetVertexBuffer:
                {
                    _gl.BindVertexBuffer(action.Slot, ((GL43Buffer) action.Buffer).Buffer, (nint) action.Offset,
                        action.Stride);
                    break;
                }
                
                case CommandListActionType.SetIndexBuffer:
                {
                    _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ((GL43Buffer) action.Buffer).Buffer);
                    break;
                }

                case CommandListActionType.SetConstantBuffer:
                {
                    _gl.BindBufferBase(BufferTargetARB.UniformBuffer, action.Slot, ((GL43Buffer) action.Buffer).Buffer);
                    break;
                }

                case CommandListActionType.SetTexture:
                {
                    GL43Texture texture = (GL43Texture) action.Texture;
                    _gl.ActiveTexture(TextureUnit.Texture0 + (int) action.Slot);
                    _gl.BindTexture(texture.Target, texture.Texture);
                    
                    break;
                }

                case CommandListActionType.DrawIndexed:
                {
                    _gl.DrawElements(PrimitiveType.Triangles, action.Slot, DrawElementsType.UnsignedInt, null);
                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Dispose() { }
}