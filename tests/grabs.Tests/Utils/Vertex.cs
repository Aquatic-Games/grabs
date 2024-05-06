using System.Numerics;

namespace grabs.Tests.Utils;

public struct Vertex
{
    public Vector3 Position;
    public Vector2 TexCoord;
    public Vector3 Normal;

    public Vertex(Vector3 position, Vector2 texCoord, Vector3 normal)
    {
        Position = position;
        TexCoord = texCoord;
        Normal = normal;
    }

    public const uint SizeInBytes = 32;
}