namespace grabs;

public abstract class Instance : IDisposable
{
    public abstract Adapter[] EnumerateAdapters();
    
    public abstract void Dispose();
}
