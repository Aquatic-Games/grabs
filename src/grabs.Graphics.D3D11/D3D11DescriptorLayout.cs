namespace grabs.Graphics.D3D11;

public sealed class D3D11DescriptorLayout : DescriptorLayout
{
    public DescriptorBindingDescription[] Bindings;

    public D3D11DescriptorLayout(in DescriptorLayoutDescription description)
    {
        Bindings = description.Bindings.ToArray();
    }

    public D3D11DescriptorLayout(DescriptorBindingDescription[] bindings)
    {
        Bindings = bindings;
    }

    public override void Dispose() { }
}