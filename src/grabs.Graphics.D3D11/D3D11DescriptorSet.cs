namespace grabs.Graphics.D3D11;

public sealed class D3D11DescriptorSet : DescriptorSet
{
    public DescriptorSetDescription[] Descriptions;

    public D3D11DescriptorSet(DescriptorSetDescription[] descriptions)
    {
        Descriptions = descriptions;
    }

    public override void Dispose() { }
}