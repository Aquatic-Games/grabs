using System.Numerics;
using System.Runtime.InteropServices;

namespace grabs.Utils;

[StructLayout(LayoutKind.Sequential)]
public struct VertexPositionTexture
{
    public Vector3 Position;
    public Vector2 TexCoord;

    public VertexPositionTexture(Vector3 position, Vector2 texCoord)
    {
        Position = position;
        TexCoord = texCoord;
    }
}