using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkSwapchain : Swapchain
{
    public override bool IsDisposed { get; protected set; }
    
    private readonly VkDevice _device;

    private readonly SwapchainKHR _swapchain;

    public VkSwapchain(Vk vk, VkDevice device, SurfaceKHR surface, ref readonly SwapchainInfo info)
    {
        ResourceManager.RegisterDeviceResource(device.Device, this);

        _device = device;

        KhrSwapchain khrSwapchain = device.KhrSwapchain;
        KhrSurface khrSurface = device.KhrSurface;

        SurfaceCapabilitiesKHR surfaceCapabilities;
        khrSurface.GetPhysicalDeviceSurfaceCapabilities(device.PhysicalDevice, surface, &surfaceCapabilities)
            .Check("Get surface capabilities");
        
        surfaceCapabilities
    }
    
    public override void Dispose()
    {
        _device.KhrSwapchain.DestroySwapchain(_device.Device, _swapchain, null);
        
        ResourceManager.DeregisterDeviceResource(_device.Device, this);
    }
}