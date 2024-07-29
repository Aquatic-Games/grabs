using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public sealed class VkSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly KhrSwapchain _khrSwapchain;
    
    public override PresentMode PresentMode { get; set; }

    public VkSwapchain(Vk vk, KhrSwapchain khrSwapchain, SurfaceKHR surface, in SwapchainDescription description)
    {
        _vk = vk;
        _khrSwapchain = khrSwapchain;

        Extent2D swapchainSize = new Extent2D(description.Width, description.Width);
        
        SwapchainCreateInfoKHR swapchainCreateInfo = new SwapchainCreateInfoKHR()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            
        }
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
        throw new System.NotImplementedException();
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}