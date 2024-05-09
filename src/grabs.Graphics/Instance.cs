using System;

namespace grabs.Graphics;

public abstract class Instance : IDisposable
{
    public abstract GraphicsApi Api { get; }
    
    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);
    
    public abstract Adapter[] EnumerateAdapters();
    
    public abstract void Dispose();
}
