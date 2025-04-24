namespace grabs.Graphics;

public interface IBackendBase
{
    public Instance CreateInstance(ref readonly InstanceInfo info);
}