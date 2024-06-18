namespace grabs.Graphics.GL43;

public sealed class GL43DescriptorLayout : DescriptorLayout
{
    public readonly DescriptorBindingDescription[] Bindings;
    
    public GL43DescriptorLayout(in DescriptorLayoutDescription description)
    {
        Bindings = description.Bindings.ToArray();
    }

    public GL43DescriptorLayout(DescriptorBindingDescription[] bindings)
    {
        Bindings = bindings;
    }

    public override void Dispose() { }
}