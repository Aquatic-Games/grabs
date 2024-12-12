using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public sealed unsafe class VulkanSurface : Surface
{
    private KhrSurface? _khrSurface;
    private VkInstance _instance;

    public SurfaceKHR Surface;

    public readonly Func<VkInstance, SurfaceKHR> CreateSurfaceFunc;

    public VulkanSurface(Func<VkInstance, SurfaceKHR> createSurfaceFunc)
    {
        _khrSurface = null;
        _instance = default;
        Surface = default;
        CreateSurfaceFunc = createSurfaceFunc;
    }

    internal void Create(KhrSurface khrSurface, VkInstance instance)
    {
        // Surface has already been created.
        if (_khrSurface != null)
            return;

        _khrSurface = khrSurface;
        _instance = instance;

        Surface = CreateSurfaceFunc(instance);
    }

    public override void Dispose()
    {
        _khrSurface?.DestroySurface(_instance, Surface, null);
    }
}