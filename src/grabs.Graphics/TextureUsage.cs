using System;

namespace grabs.Graphics;

[Flags]
public enum TextureUsage
{
    None,
    
    ShaderResource,
    
    Framebuffer,
    
    GenerateMips
}