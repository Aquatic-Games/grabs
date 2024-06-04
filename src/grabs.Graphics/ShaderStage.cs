using System;

namespace grabs.Graphics;

[Flags]
public enum ShaderStage
{
    Vertex = 0,
    Pixel = 1 << 0,
    Compute = 1 << 1,
    // TODO: Tessellation shaders
    
    All = Vertex | Pixel | Compute
}