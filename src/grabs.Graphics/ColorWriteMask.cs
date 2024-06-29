using System;

namespace grabs.Graphics;

[Flags]
public enum ColorWriteMask
{
    Red = 1 << 0,
    
    Green = 1 << 1,
    
    Blue = 1 << 2,
    
    Alpha = 1 << 3,
    
    Rgb = Red | Green | Blue,
    
    All = Red | Green | Blue | Alpha
}