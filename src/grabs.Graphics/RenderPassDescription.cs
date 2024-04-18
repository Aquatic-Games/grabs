using System.Numerics;

namespace grabs.Graphics;

public struct RenderPassDescription
{
    public ColorTarget ColorTarget1;
    public ColorTarget ColorTarget2;
    public ColorTarget ColorTarget3;
    public ColorTarget ColorTarget4;
    public ColorTarget ColorTarget5;
    public ColorTarget ColorTarget6;
    public ColorTarget ColorTarget7;
    public ColorTarget ColorTarget8;
    
    // TODO: Maybe don't use Vector4? Some custom type?
    public Vector4 ClearColor;
}