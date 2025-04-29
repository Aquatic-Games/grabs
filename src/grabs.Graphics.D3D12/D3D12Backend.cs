namespace grabs.Graphics.D3D12;

public class D3D12Backend : IBackend
{
    public static string Name => "D3D12";
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        return new D3D12Instance(in info);
    }
}