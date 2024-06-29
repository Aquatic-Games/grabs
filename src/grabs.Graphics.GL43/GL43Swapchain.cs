using System;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Swapchain : Swapchain
{
    private GL _gl;
    
    private GL43Surface _surface;
    private GL43Texture _swapchainTexture;
    
    private PresentMode _presentMode;
    private int _swapInterval;

    private Format _swapchainFormat;

    private uint _drawVao;
    private uint _drawProgram;
    
    public uint Width;
    public uint Height;
    
    public override PresentMode PresentMode
    {
        get => _presentMode;
        set
        {
            // Like DX, GL only supports Immediate and Vertical sync, so the code is just copy pasted from the DX layer.
            (_presentMode, _swapInterval) = value switch
            {
                PresentMode.Immediate => (PresentMode.Immediate, 0),
                PresentMode.VerticalSync => (PresentMode.VerticalSync, 1),
                PresentMode.AdaptiveSync => (PresentMode.VerticalSync, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public GL43Swapchain(GL gl, GL43Surface surface, in SwapchainDescription description)
    {
        _gl = gl;
        _surface = surface;

        _swapchainFormat = description.Format;
        
        Width = description.Width;
        Height = description.Height;
        
        PresentMode = description.PresentMode;

        _drawVao = gl.GenVertexArray();

        uint vShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vShader, DrawVertex);
        gl.CompileShader(vShader);
        gl.GetShader(vShader, ShaderParameterName.CompileStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to compile vertex shader: {gl.GetShaderInfoLog(vShader)}");
        
        uint fShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(fShader, DrawFragment);
        gl.CompileShader(fShader);
        gl.GetShader(fShader, ShaderParameterName.CompileStatus, out status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to compile fragment shader: {gl.GetShaderInfoLog(fShader)}");

        _drawProgram = gl.CreateProgram();
        gl.AttachShader(_drawProgram, vShader);
        gl.AttachShader(_drawProgram, fShader);
        gl.LinkProgram(_drawProgram);
        gl.GetProgram(_drawProgram, ProgramPropertyARB.LinkStatus, out status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to link program: {gl.GetProgramInfoLog(_drawProgram)}");
        gl.DetachShader(_drawProgram, vShader);
        gl.DetachShader(_drawProgram, fShader);
        gl.DeleteShader(vShader);
        gl.DeleteShader(fShader);
    }

    public override unsafe Texture GetSwapchainTexture()
    {
        _swapchainTexture = new GL43Texture(_gl,
            TextureDescription.Texture2D(Width, Height, 1, _swapchainFormat,
                TextureUsage.Framebuffer | TextureUsage.ShaderResource), null);
        
        return _swapchainTexture;
    }

    public override void Present()
    {
        _gl.Disable(EnableCap.ScissorTest);
        _gl.Disable(EnableCap.DepthTest);
        _gl.Enable(EnableCap.CullFace);
        
        _gl.FrontFace(FrontFaceDirection.CW);
        _gl.CullFace(TriangleFace.Back);
        
        _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _gl.Viewport(0, 0, Width, Height);
        _gl.ClearColor(0, 0, 0, 1);
        _gl.Clear(ClearBufferMask.ColorBufferBit);
        
        _gl.BindVertexArray(_drawVao);
        _gl.UseProgram(_drawProgram);
        
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _swapchainTexture.Texture);
        
        _gl.DrawArrays(Silk.NET.OpenGL.PrimitiveType.Triangles, 0, 6);
        
        // Scissor test is always enabled. Except here, where it is ignored.
        _gl.Enable(EnableCap.ScissorTest);
        
        _surface.PresentFunc(_swapInterval);
    }

    public override void Dispose()
    {
        _gl.DeleteProgram(_drawProgram);
        _gl.DeleteVertexArray(_drawVao);
    }

    private const string DrawVertex = """
                                      #version 330 core
                                      
                                      out vec2 frag_TexCoord;

                                      vec2 Vertices[] = vec2[](
                                          vec2(-1.0, -1.0),
                                          vec2(-1.0,  1.0),
                                          vec2( 1.0,  1.0),
                                          vec2( 1.0, -1.0)
                                      );

                                      vec2 TexCoords[] = vec2[](
                                          vec2(0.0, 0.0),
                                          vec2(0.0, 1.0),
                                          vec2(1.0, 1.0),
                                          vec2(1.0, 0.0)
                                      );
                                      
                                      int Indices[] = int[](
                                          0, 1, 3,
                                          1, 2, 3
                                      );
                                      
                                      void main() {
                                          int currentIndex = Indices[gl_VertexID];
                                      
                                          gl_Position = vec4(Vertices[currentIndex], 0.0, 1.0);
                                          frag_TexCoord = TexCoords[currentIndex];
                                      }
                                      """;

    private const string DrawFragment = """
                                        #version 330 core
                                        
                                        in vec2 frag_TexCoord;
                                        
                                        out vec4 out_Color;
                                        
                                        uniform sampler2D Texture;
                                        
                                        void main() {
                                            out_Color = texture(Texture, frag_TexCoord);
                                        }
                                        """;
}