using grabs.Core;
using grabs.Graphics.Exceptions;
using NotSupportedException = System.NotSupportedException;

namespace grabs.Graphics;

/// <summary>
/// The base class used to enumerate adapters, and create devices and surfaces.
/// </summary>
public abstract class Instance : IDisposable
{
    /// <summary>
    /// The current backend string for this instance. Compare this with <see cref="IBackend.Name"/>.
    /// </summary>
    public abstract string Backend { get; }
    
    /// <summary>
    /// Enumerate the available <see cref="Adapter"/>s present on the system.
    /// </summary>
    /// <returns>The supported <see cref="Adapter"/>s that can be used to create a <see cref="Device"/>.</returns>
    public abstract Adapter[] EnumerateAdapters();
    
    /// <summary>
    /// Create a <see cref="Device"/> which will be used for rendering.
    /// </summary>
    /// <param name="surface">A surface to use during creation.</param>
    /// <param name="adapter">An <see cref="Adapter"/> to use as the device, if any. If none are provided, the first
    /// adapter in <see cref="EnumerateAdapters"/> will be used.</param>
    /// <returns>The created <see cref="Device"/>.</returns>
    /// <remarks>The provided surface is only used during creation. It does not necessarily need to be the surface you
    /// will create a swapchain for.</remarks>
    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);

    /// <summary>
    /// Create a surface from the given info.
    /// </summary>
    /// <param name="info">The <see cref="SurfaceInfo"/> to use to create the surface.</param>
    /// <returns>The created <see cref="Surface"/>.</returns>
    public abstract Surface CreateSurface(in SurfaceInfo info);
    
    /// <summary>
    /// Dispose of this instance.
    /// </summary>
    public abstract void Dispose();

    private static Dictionary<string, IBackendBase> _backends;

    static Instance()
    {
        _backends = new Dictionary<string, IBackendBase>();
    }

    /// <summary>
    /// Register a <see cref="IBackend"/> that can be created.
    /// </summary>
    /// <typeparam name="T">Any type implementing <see cref="IBackend"/>.</typeparam>
    public static void RegisterBackend<T>() where T : IBackend, new()
    {
        _backends.Add(T.Name, new T());
    }

    public static void RegisterBackend<T>(T backend) where T : IBackend
    {
        _backends.Add(T.Name, backend);
    }

    /// <summary>
    /// Create a new GRABS <see cref="Instance"/>.
    /// </summary>
    /// <param name="info">The <see cref="InstanceInfo"/> to use when initializing the instance.</param>
    /// <returns>The created <see cref="Instance"/>.</returns>
    /// <exception cref="NoBackendsException">Thrown when no backends have been registered with <see cref="RegisterBackend{T}"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown if none of the registered backends are supported by the system.</exception>
    /// <remarks>This will attempt to initialize the backend that was registered first. If unsuccessful, it will move
    /// onto the next backend. Therefore, you should register backends in order of priority.</remarks>
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