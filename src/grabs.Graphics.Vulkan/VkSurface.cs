using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

public class VkSurface : Surface
{
    public SurfaceKHR Surface;

    public VkSurface(SurfaceKHR surface)
    {
        Surface = surface;
    }

    public override void Dispose()
    {
        
    }
}