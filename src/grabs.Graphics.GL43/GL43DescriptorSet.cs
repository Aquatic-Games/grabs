namespace grabs.Graphics.GL43;

public sealed class GL43DescriptorSet : DescriptorSet
{
    public readonly DescriptorBindingDescription[] Bindings;

    public readonly DescriptorSetDescription[] Descriptions;

    public GL43DescriptorSet(DescriptorBindingDescription[] bindings, DescriptorSetDescription[] descriptions)
    {
        Bindings = bindings;
        Descriptions = descriptions;
    }
    
    public override void Dispose() { }
}