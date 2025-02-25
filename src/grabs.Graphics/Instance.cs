using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using grabs.Graphics.Exceptions;
using NotSupportedException = System.NotSupportedException;

namespace grabs.Graphics;

public abstract class Instance : IDisposable
{
    public abstract string Backend { get; }
    
    public abstract Adapter[] EnumerateAdapters();
    
    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);

    public abstract Surface CreateSurface(in SurfaceInfo info);
    
    public abstract void Dispose();

    private static Dictionary<string, IBackendBase> _backends;

    static Instance()
    {
        _backends = new Dictionary<string, IBackendBase>();
    }

    public static void RegisterBackend<T>() where T : IBackend, new()
    {
        _backends.Add(T.Name, new T());
    }

    public static Instance Create(in InstanceInfo info)
    {
        if (_backends.Count == 0)
            throw new NoBackendsException();
        
        GrabsLog.Log($"Registered backends: {string.Join(", ", _backends.Keys)}");

        foreach ((string name, IBackendBase backend) in _backends)
        {
            try
            {
                return backend.CreateInstance(in info);
            }
            catch (Exception e)
            {
                GrabsLog.Log(GrabsLog.Severity.Error, GrabsLog.Type.Other, $"Failed to create {name} instance: {e}");
            }
        }

        throw new NotSupportedException("None of the provided backends were supported by this system.");
    }
}