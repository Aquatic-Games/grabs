namespace grabs;

public abstract class Instance : IDisposable
{
    public abstract void Dispose();

    public static Instance Create(in InstanceInfo info)
    {
        throw new NotImplementedException();
    }
}