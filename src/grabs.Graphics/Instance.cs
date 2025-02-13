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
            backend = Backend.Vulkan;

        if (backend.HasFlag(Backend.Vulkan))
            return new VulkanInstance(in info);

        throw new NotImplementedException();
    }
}