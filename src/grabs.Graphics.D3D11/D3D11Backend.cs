namespace grabs.Graphics.D3D11;

public class D3D11Backend : IBackend
{
    public static string Name => "DirectX 11";
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        return new D3D11Instance(in info);
    }
}