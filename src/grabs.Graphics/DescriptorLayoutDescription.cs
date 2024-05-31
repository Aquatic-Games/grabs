using System;

namespace grabs.Graphics;

public ref struct DescriptorLayoutDescription
{
    public ReadOnlySpan<DescriptorBindingDescription> Bindings;

    public DescriptorLayoutDescription(params DescriptorBindingDescription[] bindings)
    {
        Bindings = bindings.AsSpan();
    }

    public DescriptorLayoutDescription(ReadOnlySpan<DescriptorBindingDescription> bindings)
    {
        Bindings = bindings;
    }
}