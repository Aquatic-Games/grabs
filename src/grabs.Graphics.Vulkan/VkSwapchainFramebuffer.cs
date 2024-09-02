using System;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

public class VkSwapchainFramebuffer : Framebuffer
{
    public ImageView[] Views;

    public VkSwapchainFramebuffer(VkSwapchainTexture texture)
    {
        Views = texture.Views;
    }
    
    public override void Dispose()
    {
        
    }
}