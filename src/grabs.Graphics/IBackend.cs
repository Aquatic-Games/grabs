namespace grabs.Graphics;

public interface IBackend
{
    public static abstract string Name { get; }

    public Instance CreateInstance(ref readonly InstanceInfo info);
}