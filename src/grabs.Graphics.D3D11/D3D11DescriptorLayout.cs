namespace grabs.Graphics.D3D11;

internal sealed class D3D11DescriptorLayout : DescriptorLayout
{
    public readonly DescriptorBinding[] Bindings;
    
    public D3D11DescriptorLayout(ref readonly DescriptorLayoutInfo info)
    {
        Bindings = info.Bindings.ToArray();
    }
    
    public override void Dispose()
    {
        // Nothing to dispose.
    }
}