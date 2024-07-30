using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using static grabs.Graphics.Vulkan.VkResult;

namespace grabs.Graphics.Vulkan;

public sealed unsafe class VkSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    private readonly Queue _presentQueue;
    private readonly KhrSwapchain _khrSwapchain;

    public readonly SwapchainKHR Swapchain;
    
    public override PresentMode PresentMode { get; set; }

    public VkSwapchain(Vk vk, VkDevice device, SurfaceKHR surface, in SwapchainDescription description)
    {
        _vk = vk;
        _device = device.Device;
        _presentQueue = device.PresentQueue;
        _khrSwapchain = device.KhrSwapchain;

        SwapchainCreateInfoKHR swapchainCreateInfo = new SwapchainCreateInfoKHR()
        {
            SType = StructureType.SwapchainCreateInfoKhr,

            Surface = surface,
            MinImageCount = description.BufferCount,
            ImageFormat = VkUtils.FormatToVk(description.Format),
            ImageColorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr,
            ImageExtent = new Extent2D(description.Width, description.Height),
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PresentMode = VkUtils.PresentModeToVk(description.PresentMode),
            Clipped = true
        };

        uint* queueFamilies = stackalloc uint[] { device.GraphicsQueueIndex, device.PresentQueueIndex };
        
        if (device.GraphicsQueueIndex != device.PresentQueueIndex)
        {
            swapchainCreateInfo.ImageSharingMode = SharingMode.Concurrent;
            swapchainCreateInfo.QueueFamilyIndexCount = 2;
            swapchainCreateInfo.PQueueFamilyIndices = queueFamilies;
        }
        
        CheckResult(_khrSwapchain.CreateSwapchain(_device, &swapchainCreateInfo, null, out Swapchain), "Create swapchain");
    }
    
    public override Texture GetSwapchainTexture()
    {
        throw new System.NotImplementedException();
    }

    public override void Resize(uint width, uint height)
    {
        throw new System.NotImplementedException();
    }

    public override void Present()
    {
        PresentInfoKHR presentInfo = new PresentInfoKHR()
        {
            SType = StructureType.PresentInfoKhr,
            
        }
        
        _khrSwapchain.QueuePresent(_presentQueue, )
    }

    public override void Dispose()
    {
        _khrSwapchain.DestroySwapchain(_device, Swapchain, null);
    }
}