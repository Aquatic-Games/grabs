using System.Numerics;
using System.Runtime.InteropServices;

namespace grabs.Utils;

[StructLayout(LayoutKind.Sequential)]
public struct VertexPositionColor
{
    public Vector3 Position;
    public Vector4 Color;

    public VertexPositionColor(Vector3 position, Vector4 color)
    {
        Position = position;
        Color = color;
    }
}