using grabs.Vulkan;

namespace grabs;

public abstract class Instance : IDisposable
{
    public abstract Adapter[] EnumerateAdapters();
    
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