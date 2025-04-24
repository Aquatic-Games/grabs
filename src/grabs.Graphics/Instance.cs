using System.Diagnostics;

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
        
        foreach ((_, IBackendBase backend) in _backends)
        {
            //try
            {
                return backend.CreateInstance(in info);
            }
            //catch (Exception e) { }
        }

        throw new PlatformNotSupportedException("No backends were supported by this platform.");
    }
}
