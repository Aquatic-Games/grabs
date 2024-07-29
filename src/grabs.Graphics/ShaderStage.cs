using System;

namespace grabs.Graphics;

[Flags]
public enum ShaderStage
{
    Vertex = 1 << 0,
    Pixel = 1 << 1,
    Compute = 1 << 2,
    // TODO: Tessellation shaders
    
    VertexPixel = Vertex | Pixel,
    All = Vertex | Pixel | Compute
}