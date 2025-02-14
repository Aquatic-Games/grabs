using grabs.Graphics.D3D11;
using grabs.Graphics.Vulkan;

namespace grabs.Graphics;

public abstract class Instance : IDisposable
{
    public abstract Backend Backend { get; }
    
    public abstract Adapter[] EnumerateAdapters();
    
    public abstract Device CreateDevice(Surface surface, Adapter? adapter = null);

    public abstract Surface CreateSurface(in SurfaceInfo info);
    
    public abstract void Dispose();

    public static Instance Create(in InstanceInfo info)
    {
        Backend backend = info.BackendHint;
        if (backend == Backend.Unknown)
            backend = Backend.Vulkan | Backend.D3D11;

        // Prioritize D3D11 on Windows.
        if (OperatingSystem.IsWindows() && backend.HasFlag(Backend.D3D11))
            return new D3D11Instance(in info);

        if (backend.HasFlag(Backend.Vulkan))
            return new VulkanInstance(in info);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Check to see if the given backend is supported.
    /// </summary>
    /// <param name="backend">The <see cref="grabs.Graphics.Backend"/> to check.</param>
    /// <param name="withDebugging">Inserts the backend's debug utils.</param>
    /// <returns>true, if the backend is supported.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If an invalid, or unknown <see cref="grabs.Graphics.Backend"/> is provided.</exception>
    /// <remarks>This is a relatively expensive operation, as it has to create, then destroy an <see cref="Instance"/>.
    /// It's recommended you only do the check once and store the result, or do the check via other means.</remarks>
    public static bool IsBackendSupported(Backend backend, bool withDebugging = false)
    {
        switch (backend)
        {
            case Backend.Vulkan:
            {
                try
                {
                    Instance instance = Create(new InstanceInfo(Backend.Vulkan, "", withDebugging));
                    instance.EnumerateAdapters();
                    
                    instance.Dispose();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            case Backend.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(backend), backend, null);
        }
    }
}