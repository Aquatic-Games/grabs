namespace grabs.Graphics.D3D11;

public sealed class D3D11Backend : IBackend
{
    public static string Name => "D3D11";

    public Instance CreateInstance(ref readonly InstanceInfo info)
        => new D3D11Instance(in info);
}