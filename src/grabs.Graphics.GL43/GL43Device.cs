using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public class GL43Device : Device
{
    private readonly GL _gl;
    private GL43Surface _surface;
    private GL43Swapchain _swapchain;

    private GL43Pipeline _currentPipeline;
    private GL43DescriptorSet[] _setSets;

    private DrawElementsType _currentDrawElementsType;
    private uint _currentDrawElementsSizeInBytes;
    private Silk.NET.OpenGL.PrimitiveType _currentPrimitiveType;
    
    public GL43Device(GL gl, GL43Surface surface)
    {
        _gl = gl;
        _surface = surface;

        _setSets = new GL43DescriptorSet[16];
        
        // Scissor test is always enabled.
        _gl.Enable(EnableCap.ScissorTest);
    }
    
    public override Swapchain CreateSwapchain(in SwapchainDescription description)
    {
        // Unfortunately we have to store a reference to the swapchain, so we can get its size, which are used in the
        // viewport and scissor commands.
        _swapchain = new GL43Swapchain(_gl, _surface, description);
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
        return new GL43Framebuffer(_gl, colorTextures, depthTexture);
    }

    public override DescriptorLayout CreateDescriptorLayout(in DescriptorLayoutDescription description)
    {
        return new GL43DescriptorLayout(description);
    }

    public override DescriptorSet CreateDescriptorSet(DescriptorLayout layout, in ReadOnlySpan<DescriptorSetDescription> descriptions)
    {
        return new GL43DescriptorSet(descriptions.ToArray());
    }

    public override void UpdateDescriptorSet(DescriptorSet set, in ReadOnlySpan<DescriptorSetDescription> descriptions)
    {
        GL43DescriptorSet glSet = (GL43DescriptorSet) set;
        glSet.Descriptions = descriptions.ToArray();
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
                    
                    _gl.BindFramebuffer(FramebufferTarget.Framebuffer, ((GL43Framebuffer) desc.Framebuffer).Framebuffer);

                    ClearBufferMask mask = ClearBufferMask.None;

                    if (desc.ColorLoadOp == LoadOp.Clear)
                    {
                        _gl.ClearColor(desc.ClearColor.X, desc.ClearColor.Y, desc.ClearColor.Z, desc.ClearColor.W);
                        mask |= ClearBufferMask.ColorBufferBit;
                    }

                    if (desc.DepthLoadOp == LoadOp.Clear)
                    {
                        _gl.ClearDepth(desc.DepthValue);
                        mask |= ClearBufferMask.DepthBufferBit;
                    }

                    if (desc.StencilLoadOp == LoadOp.Clear)
                    {
                        _gl.ClearStencil(desc.StencilValue);
                        mask |= ClearBufferMask.StencilBufferBit;
                    }
                    
                    if (mask != ClearBufferMask.None)
                        _gl.Clear(mask);
                    
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
                    _gl.DepthRange(viewport.MinDepth, viewport.MaxDepth);
                    break;
                }

                case CommandListActionType.SetScissor:
                {
                    Viewport rect = action.Viewport;
                    _gl.Scissor(rect.X, (int) _swapchain.Height - rect.Y - (int) rect.Height, rect.Width, rect.Height);
                    break;
                }

                case CommandListActionType.SetPipeline:
                {
                    GL43Pipeline pipeline = (GL43Pipeline) action.Pipeline;
                    _currentPipeline = pipeline;
                    
                    _gl.BindVertexArray(pipeline.Vao);
                    _gl.UseProgram(pipeline.ShaderProgram);

                    DepthStencilDescription depthDesc = pipeline.DepthStencilDescription;
                    RasterizerDescription rasterizerDesc = pipeline.RasterizerDescription;
                    BlendDescription blendDesc = pipeline.BlendDescription;

                    if (depthDesc.DepthEnabled)
                    {
                        _gl.Enable(EnableCap.DepthTest);
                        _gl.DepthMask(depthDesc.DepthWrite);

                        DepthFunction depthFunc = depthDesc.ComparisonFunction switch
                        {
                            ComparisonFunction.Never => DepthFunction.Never,
                            ComparisonFunction.Less => DepthFunction.Less,
                            ComparisonFunction.Equal => DepthFunction.Equal,
                            ComparisonFunction.LessEqual => DepthFunction.Lequal,
                            ComparisonFunction.Greater => DepthFunction.Greater,
                            ComparisonFunction.NotEqual => DepthFunction.Notequal,
                            ComparisonFunction.GreaterEqual => DepthFunction.Gequal,
                            ComparisonFunction.Always => DepthFunction.Always,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        _gl.DepthFunc(depthFunc);
                    }
                    else
                        _gl.Disable(EnableCap.DepthTest);
                    
                    if (rasterizerDesc.CullFace == CullFace.None)
                        _gl.Disable(EnableCap.CullFace);
                    else
                    {
                        _gl.Enable(EnableCap.CullFace);
                        _gl.CullFace(rasterizerDesc.CullFace == CullFace.Front
                            ? TriangleFace.Front
                            : TriangleFace.Back);
                        _gl.FrontFace(rasterizerDesc.FrontFace == CullDirection.Clockwise
                            ? FrontFaceDirection.CW
                            : FrontFaceDirection.Ccw);
                    }
                    
                    if (blendDesc.IndependentBlending)
                    {
                        for (uint i = 0; i < blendDesc.Attachments.Length; i++)
                        {
                            ref BlendAttachmentDescription attachmentDesc = ref blendDesc.Attachments[i];

                            if (!attachmentDesc.Enabled)
                            {
                                _gl.Disable(EnableCap.Blend, i);
                                continue;
                            }
                            
                            _gl.Enable(EnableCap.Blend, i);

                            _gl.BlendFuncSeparate(i, GLUtils.BlendFactorToGL(attachmentDesc.Source),
                                GLUtils.BlendFactorToGL(attachmentDesc.Destination),
                                GLUtils.BlendFactorToGL(attachmentDesc.SourceAlpha),
                                GLUtils.BlendFactorToGL(attachmentDesc.DestinationAlpha));

                            _gl.BlendEquationSeparate(i, GLUtils.BlendOperationToGL(attachmentDesc.BlendOperation),
                                GLUtils.BlendOperationToGL(attachmentDesc.AlphaBlendOperation));
                        }
                        
                        // For independent blend, we always set the blend color, as its easier than checking if every
                        // blend is disabled or not.
                        _gl.BlendColor(blendDesc.BlendConstants.X, blendDesc.BlendConstants.Y,
                            blendDesc.BlendConstants.Z, blendDesc.BlendConstants.W);
                    }
                    else
                    {
                        ref BlendAttachmentDescription attachmentDesc = ref blendDesc.Attachments[0];

                        if (attachmentDesc.Enabled)
                        {
                            _gl.Enable(EnableCap.Blend);
                            _gl.BlendColor(blendDesc.BlendConstants.X, blendDesc.BlendConstants.Y,
                                blendDesc.BlendConstants.Z, blendDesc.BlendConstants.W);
                            
                            _gl.BlendFuncSeparate(GLUtils.BlendFactorToGL(attachmentDesc.Source),
                                GLUtils.BlendFactorToGL(attachmentDesc.Destination),
                                GLUtils.BlendFactorToGL(attachmentDesc.SourceAlpha),
                                GLUtils.BlendFactorToGL(attachmentDesc.DestinationAlpha));

                            _gl.BlendEquationSeparate(GLUtils.BlendOperationToGL(attachmentDesc.BlendOperation),
                                GLUtils.BlendOperationToGL(attachmentDesc.AlphaBlendOperation));
                        }
                        else
                            _gl.Disable(EnableCap.Blend);
                    }

                    _currentPrimitiveType = pipeline.PrimitiveType;
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

                    _currentDrawElementsType = action.Format switch
                    {
                        Format.R8_UInt => DrawElementsType.UnsignedByte,
                        Format.R16_UInt => DrawElementsType.UnsignedShort,
                        Format.R32_UInt => DrawElementsType.UnsignedInt,
                        _ => throw new NotSupportedException()
                    };

                    // TODO: Should this method be renamed?
                    _currentDrawElementsSizeInBytes = action.Format.BitsPerPixel() / 8;
                    
                    break;
                }

                case CommandListActionType.SetDescriptor:
                {
                    GL43DescriptorSet set = action.DescriptorSet;
                    _setSets[action.Slot] = set;
                    
                    break;
                }

                case CommandListActionType.Draw:
                {
                    SetPreDrawParameters();
                    _gl.DrawArrays(_currentPrimitiveType, 0, action.Slot);
                    break;
                }

                case CommandListActionType.DrawIndexed:
                {
                    SetPreDrawParameters();
                    _gl.DrawElements(_currentPrimitiveType, action.Slot, _currentDrawElementsType, null);
                    break;
                }

                case CommandListActionType.DrawIndexedBaseVertex:
                {
                    SetPreDrawParameters();
                    _gl.DrawElementsBaseVertex(_currentPrimitiveType, action.Slot, _currentDrawElementsType,
                        (void*) (action.Offset * _currentDrawElementsSizeInBytes), (int) action.Stride);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void SetPreDrawParameters()
    {
        if (_currentPipeline.Layouts == null)
            return;
        
        for (int i = 0; i < _currentPipeline.Layouts.Length; i++)
        {
            GL43DescriptorLayout layout = _currentPipeline.Layouts[i];

            for (int j = 0; j < layout.Bindings.Length; j++)
            {
                ref DescriptorBindingDescription binding = ref layout.Bindings[j];
                ref DescriptorSetDescription desc = ref _setSets[i].Descriptions[j];

                switch (binding.Type)
                {
                    case DescriptorType.ConstantBuffer:
                        _gl.BindBufferBase(BufferTargetARB.UniformBuffer, binding.Binding, ((GL43Buffer) desc.Buffer).Buffer);
                        break;
                    case DescriptorType.Texture:
                        GL43Texture texture = (GL43Texture) desc.Texture;
                                
                        _gl.ActiveTexture(TextureUnit.Texture0 + (int) binding.Binding);
                        _gl.BindTexture(texture.Target, texture.Texture);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public override void Dispose() { }
}