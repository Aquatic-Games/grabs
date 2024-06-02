using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

public class VkSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly KhrSwapchain _khrSwapchain;
    
    public override PresentMode PresentMode { get; set; }

    public VkSwapchain(Vk vk, KhrSwapchain khrSwapchain, VulkanDevice device, in SwapchainDescription description)
    {
        _vk = vk;
        _khrSwapchain = khrSwapchain;
    }
    
    public override Texture GetSwapchainTexture()
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