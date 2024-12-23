using grabs.Graphics.D3D11;
using grabs.Graphics.Exceptions;
using grabs.Graphics.LegacyGL;
using grabs.Graphics.Vulkan;

namespace grabs.Graphics;

/// <summary>
/// An instance contains the base methods to create and manage <see cref="Device"/>s.
/// </summary>
public abstract class Instance : IDisposable
{
    /// <summary>
    /// Check to see if this <see cref="Instance"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The current <see cref="grabs.Graphics.Backend"/> for this <see cref="Instance"/>.
    /// </summary>
    public abstract Backend Backend { get; }

    /// <summary>
    /// Enumerate through all supported <see cref="Adapter"/>s present in the system.
    /// </summary>
    /// <returns>A list of <see cref="Adapter"/>s.</returns>
    public abstract Adapter[] EnumerateAdapters();

    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);

    public static Instance Create(in InstanceDescription description, IWindowProvider windowProvider)
    {
        Backend backendHint = description.BackendHint == Backend.Unknown
            ? Backend.Vulkan | Backend.D3D11
            : description.BackendHint;

        if (OperatingSystem.IsWindows())
        {
            if (backendHint.HasFlag(Backend.D3D11))
                return new D3D11Instance(description.Debug);
        }

        if (backendHint.HasFlag(Backend.Vulkan))
            return new VulkanInstance(description.Debug, windowProvider);

        if (backendHint.HasFlag(Backend.LegacyGL))
            return new GLInstance(windowProvider.GetGLProcAddress);

        throw new NoBackendException();
    }
    
    public abstract void Dispose();
}