using System.Diagnostics;
using grabs.Core;
using grabs.Graphics.Debugging;

namespace grabs.Graphics;

/// <summary>
/// The base instance, used to create devices.
/// </summary>
public abstract class Instance : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Instance"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// Get the name of this backend.
    /// </summary>
    public abstract string BackendName { get; }

    /// <summary>
    /// Enumerate the graphics adapters available on this system.
    /// </summary>
    /// <returns>An array of supported <see cref="Adapter"/>s.</returns>
    public abstract Adapter[] EnumerateAdapters();

    /// <summary>
    /// Create a <see cref="Surface"/>.
    /// </summary>
    /// <param name="info">The <see cref="SurfaceInfo"/> that describes the surface.</param>
    /// <returns>The created <see cref="Surface"/>.</returns>
    public abstract Surface CreateSurface(in SurfaceInfo info);

    /// <summary>
    /// Create a logical <see cref="Device"/>.
    /// </summary>
    /// <param name="surface">The <see cref="Surface"/> to use during device creation.</param>
    /// <param name="adapter">The <see cref="Adapter"/> this device will be created on. If none is provided, the default
    /// will be used.</param>
    /// <returns>The created <see cref="Device"/>.</returns>
    /// <remarks>The <paramref name="surface"/> is only used during creation. It does <b>not</b> need to be the same
    /// surface used to create a <see cref="Swapchain"/>.</remarks>
    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);

    /// <summary>
    /// Dispose of this <see cref="Instance"/>.
    /// </summary>
    public abstract void Dispose();

    private static Dictionary<string, IBackendBase> _backends;

    static Instance()
    {
        _backends = [];
    }

    /// <summary>
    /// Register a backend that can be used.
    /// </summary>
    /// <typeparam name="T">A backend implementing <see cref="IBackend"/>.</typeparam>
    public static void RegisterBackend<T>() where T : IBackend, new()
    {
        _backends.Add(T.Name, new T());
    }
    
    /// <summary>
    /// Create an <see cref="Instance"/>.
    /// </summary>
    /// <param name="info">The <see cref="InstanceInfo"/> used to describe this instance.</param>
    /// <returns>The created <see cref="Instance"/>.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if none of the registered backends are supported by the
    /// platform.</exception>
    /// <remarks>At least 1 backend must be registered before you can call this method.</remarks>
    public static Instance Create(in InstanceInfo info)
    {
        Debug.Assert(_backends.Count > 0, "At least 1 backend must be registered!");
        
        GrabsLog.Log(GrabsLog.Severity.Info, $"Registered backends: {string.Join(", ", _backends.Keys)}");
        
        foreach ((string name, IBackendBase backend) in _backends)
        {
            //try
            {
                Instance instance = backend.CreateInstance(in info);
                return info.Debug ? new DebugInstance(instance) : instance;
            }
            //catch (Exception e)
            //{
            //    GrabsLog.Log(GrabsLog.Severity.Error, $"Failed to create backend '{name}': {e}");
            //}
        }

        throw new PlatformNotSupportedException("No backends were supported by this platform.");
    }
}
