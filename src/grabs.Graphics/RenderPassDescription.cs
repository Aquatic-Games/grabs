using System.Numerics;

namespace grabs.Graphics;

public struct RenderPassDescription
{
    public Framebuffer Framebuffer;
    
    // TODO: Maybe don't use Vector4? Some custom type?
    public Vector4 ClearColor;
    public LoadOp ColorLoadOp;

    public float DepthValue;
    public LoadOp DepthLoadOp;

    public byte StencilValue;
    public LoadOp StencilLoadOp;
    
    public RenderPassDescription(Framebuffer framebuffer, Vector4 clearColor, LoadOp colorLoadOp = LoadOp.Clear,
        float depthValue = 1.0f, LoadOp depthLoadOp = LoadOp.Clear, byte stencilValue = byte.MaxValue,
        LoadOp stencilLoadOp = LoadOp.Clear)
    {
        Framebuffer = framebuffer;
        ClearColor = clearColor;
        ColorLoadOp = colorLoadOp;
        DepthValue = depthValue;
        DepthLoadOp = depthLoadOp;
        StencilValue = stencilValue;
        StencilLoadOp = stencilLoadOp;
    }
}