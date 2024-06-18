namespace grabs.Graphics.GL43;

public sealed class GL43DescriptorSet : DescriptorSet
{
    public DescriptorSetDescription[] Descriptions;

    public GL43DescriptorSet(DescriptorSetDescription[] descriptions)
    {
        Descriptions = descriptions;
    }
    
    public override void Dispose() { }
}