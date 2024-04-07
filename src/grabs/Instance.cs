namespace grabs;

public abstract class Instance : IDisposable
{
    public abstract Device CreateDevice(Adapter? adapter = null);
    
    public abstract Adapter[] EnumerateAdapters();
    
    public abstract void Dispose();
}
