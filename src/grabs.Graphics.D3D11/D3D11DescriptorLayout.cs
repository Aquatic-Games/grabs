namespace grabs.Graphics.D3D11;

public class D3D11DescriptorLayout : DescriptorLayout
{
    public DescriptorBindingDescription[] Bindings;

    public D3D11DescriptorLayout(in DescriptorLayoutDescription description)
    {
        Bindings = description.Bindings.ToArray();
    }

    public override void Dispose() { }
}