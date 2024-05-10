using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public class VkSurface : Surface
{
    private KhrSurface _khrSurface;
    private Silk.NET.Vulkan.Instance _instance;
    
    public SurfaceKHR Surface;

    public VkSurface(SurfaceKHR surface, VkInstance instance)
    {
        Surface = surface;
        _khrSurface = instance.KhrSurface;
        _instance = instance.Instance;
    }

    public override unsafe void Dispose()
    {
        _khrSurface.DestroySurface(_instance, Surface, null);
    }
}