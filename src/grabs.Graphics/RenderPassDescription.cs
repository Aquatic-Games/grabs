using System.Numerics;

namespace grabs.Graphics;

public struct RenderPassDescription
{
    public Framebuffer Framebuffer;
    
    // TODO: Maybe don't use Vector4? Some custom type?
    public Vector4 ClearColor;

    public LoadOp LoadOp;

    public RenderPassDescription(Framebuffer framebuffer, Vector4 clearColor, LoadOp loadOp = LoadOp.Clear)
    {
        Framebuffer = framebuffer;
        ClearColor = clearColor;
        LoadOp = loadOp;
    }
}