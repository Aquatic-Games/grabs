namespace grabs.Graphics;

public ref struct DescriptorLayoutInfo
{
    public ReadOnlySpan<DescriptorBinding> Bindings;

    public DescriptorLayoutInfo(in ReadOnlySpan<DescriptorBinding> bindings)
    {
        Bindings = bindings;
    }

    public DescriptorLayoutInfo(in DescriptorBinding binding)
    {
        Bindings = new ReadOnlySpan<DescriptorBinding>(in binding);
    }
}