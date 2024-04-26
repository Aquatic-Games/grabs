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
            gl.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + i,
                ((GL43Texture) colorTextures[i]).Texture, 0);
        }

        if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != (GLEnum) FramebufferStatus.Complete)
            throw new Exception("Framebuffer is not complete.");
    }
    
    public override void Dispose()
    {
        _gl.DeleteFramebuffer(Framebuffer);
    }
}