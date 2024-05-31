namespace grabs.Graphics.D3D11;

public sealed class D3D11DescriptorSet : DescriptorSet
{
    public DescriptorBindingDescription[] Bindings;

    public DescriptorSetDescription[] Descriptions;

    public D3D11DescriptorSet(DescriptorBindingDescription[] bindings, DescriptorSetDescription[] descriptions)
    {
        Bindings = bindings;
        Descriptions = descriptions;
    }

    public override void Dispose() { }
}