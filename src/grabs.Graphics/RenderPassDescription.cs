using System.Numerics;

namespace grabs.Graphics;

public struct RenderPassDescription
{
    public Framebuffer Framebuffer;
    
    // TODO: Maybe don't use Vector4? Some custom type?
    public Vector4 ClearColor;

    public RenderPassDescription(Framebuffer framebuffer, Vector4 clearColor)
    {
        Framebuffer = framebuffer;
        ClearColor = clearColor;
    }
}