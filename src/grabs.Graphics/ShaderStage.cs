using System;

namespace grabs.Graphics;

[Flags]
public enum ShaderStage
{
    Vertex,
    Pixel,
    Compute,
    // TODO: Tessellation shaders
    
    All = Vertex | Pixel | Compute
}