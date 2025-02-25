using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using grabs.Graphics.D3D11;

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
        GrabsLog.Log($"Registered backends: {string.Join(", ", _backends.Keys)}");

        foreach ((_, IBackendBase backend) in _backends)
        {
            try
            {
                return backend.CreateInstance(in info);
            }
            catch (Exception e)
            {
                GrabsLog.Log(GrabsLog.Severity.Error, $"Failed to create instance: {e}");
            }
        }

        throw new NotImplementedException();
    }
}