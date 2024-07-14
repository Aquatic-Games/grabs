using System;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public class VkSurface : Surface
{
    public readonly VkInstance Instance;
    public readonly SurfaceKHR Surface;
    public readonly KhrSurface KhrSurface;
    
    public VkSurface(VkInstance instance, SurfaceKHR surface)
    {
        Instance = instance;
        
        if (!instance.Vk.TryGetInstanceExtension(instance.Instance, out KhrSurface))
            throw new Exception("Failed to get KHR_Surface extension.");

        Surface = surface;
    }
    
    public override unsafe void Dispose()
    {
        KhrSurface.DestroySurface(Instance.Instance, Surface, null);
    }
}