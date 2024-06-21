using System;

namespace grabs.Graphics;

[Flags]
public enum ShaderStage
{
    Vertex = 1,
    Pixel = 1 << 2,
    Compute = 1 << 3,
    // TODO: Tessellation shaders
    
    All = Vertex | Pixel | Compute
}