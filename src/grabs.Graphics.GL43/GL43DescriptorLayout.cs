namespace grabs.Graphics.GL43;

public sealed class GL43DescriptorLayout : DescriptorLayout
{
    public readonly DescriptorBindingDescription[] Bindings;
    
    public GL43DescriptorLayout(in DescriptorLayoutDescription description)
    {
        Bindings = description.Bindings.ToArray();
    }
    
    public override void Dispose() { }
}