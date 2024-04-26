using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public class GL43Framebuffer : Framebuffer
{
    private GL _gl;

    public uint Framebuffer;

    public GL43Framebuffer(GL gl, in ReadOnlySpan<Texture> colorTextures, Texture depthTexture)
    {
        _gl = gl;

        Framebuffer = gl.GenFramebuffer();
        gl.BindFramebuffer(FramebufferTarget.Framebuffer, Framebuffer);

        for (int i = 0; i < colorTextures.Length; i++)
        {
            GL43Texture glTexture = (GL43Texture) colorTextures[i];
            if (glTexture.IsRenderbuffer)
            {
                gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i,
                    RenderbufferTarget.Renderbuffer, glTexture.Texture);
            }
            else
            {
                gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i,
                    glTexture.Texture, 0);
            }
        }

        if (depthTexture != null)
        {
            GL43Texture glDepth = (GL43Texture) depthTexture;
            if (glDepth.IsRenderbuffer)
            {
                gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer, glDepth.Texture);
            }
            else
            {
                gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    glDepth.Texture, 0);
            }
        }

        if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != (GLEnum) FramebufferStatus.Complete)
            throw new Exception("Framebuffer is not complete.");
    }
    
    public override void Dispose()
    {
        _gl.DeleteFramebuffer(Framebuffer);
    }
}