using System.Numerics;

namespace grabs;

public ref struct RenderPassDescription
{
    public ReadOnlySpan<ColorTarget> ColorTargets;
    
    // TODO: Maybe don't use Vector4? Some custom type?
    public Vector4 ClearColor;

    public RenderPassDescription(ReadOnlySpan<ColorTarget> colorTargets, Vector4 clearColor)
    {
        ColorTargets = colorTargets;
        ClearColor = clearColor;
    }
}