using System;

namespace grabs.Graphics;

[Flags]
public enum DescriptorType
{
    ConstantBuffer,
    Image,
    Sampler,
    Texture
}