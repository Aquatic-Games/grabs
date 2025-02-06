using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanSwapchain : Swapchain
{
    private readonly Vk _vk;
    
    public readonly SwapchainKHR Swapchain;

    public VulkanSwapchain(Vk vk, PhysicalDevice physicalDevice, VkDevice device, KhrSurface khrSurface,
        KhrSwapchain khrSwapchain, VulkanSurface surface, ref readonly SwapchainInfo info)
    {
        _vk = vk;
        
        SurfaceCapabilitiesKHR surfaceCapabilities;
        khrSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface.Surface, &surfaceCapabilities);

        uint desiredNumImages = info.NumBuffers;
        if (desiredNumImages < surfaceCapabilities.MinImageCount)
        {
            GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Source.General,
                $"Desired buffer count ({desiredNumImages}) is under the minimum allowed buffer count ({surfaceCapabilities.MinImageCount}). Clamping to the minimum.");

            desiredNumImages = surfaceCapabilities.MinImageCount;
        }
        // If max image count is 0, there is no limit.
        else if (desiredNumImages > surfaceCapabilities.MaxImageCount && surfaceCapabilities.MaxImageCount != 0)
        {
            GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Source.General,
                $"Desired buffer count ({desiredNumImages}) is above the maximum allowed buffer count ({surfaceCapabilities.MaxImageCount}). Clamping to the maximum.");

            desiredNumImages = surfaceCapabilities.MaxImageCount;
        }
        
        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General,
            "Checking supported present modes over selected present mode.");
        
        uint numPresentModes;
        khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface.Surface, &numPresentModes, null);
        PresentModeKHR* presentModes = stackalloc PresentModeKHR[(int) numPresentModes];
        khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface.Surface, &numPresentModes, null);

        PresentModeKHR desiredPresentMode = info.PresentMode switch
        {
            PresentMode.Immediate => PresentModeKHR.ImmediateKhr,
            PresentMode.Mailbox => PresentModeKHR.MailboxKhr,
            PresentMode.Fifo => PresentModeKHR.FifoKhr,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        for (uint i = 0; i < numPresentModes; i++)
        {
            if (desiredPresentMode == presentModes[i])
                goto PRESENT_MODE_FOUND;
        }

        GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Source.Performance,
            $"Desired present mode {desiredPresentMode} is not supported by the GPU - present mode {presentModes[0]} will be used instead.");

        desiredPresentMode = presentModes[0];
        
        PRESENT_MODE_FOUND: ;

        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General,
            "Checking supported formats over selected format.");

        uint numFormats;
        khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface.Surface, &numFormats, null);
        SurfaceFormatKHR* formats = stackalloc SurfaceFormatKHR[(int) numFormats];
        khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface.Surface, &numFormats, formats);

        VkFormat desiredFormat = info.Format.ToVk();

        for (uint i = 0; i < numFormats; i++)
        {
            if (desiredFormat == formats[i].Format && formats[i].ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
                goto FORMAT_FOUND;
        }
        
        GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Source.Performance,
            $"Desired format {desiredFormat} is not supported by the GPU - format {formats[0].Format} will be used instead.");

        desiredFormat = formats[0].Format;
        
        FORMAT_FOUND: ;

        // TODO: Compare desiredExtent vs the Min and Max extents.
        Extent2D desiredExtent = new Extent2D(info.Width, info.Height);
        
        SwapchainCreateInfoKHR swapchainInfo = new SwapchainCreateInfoKHR()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            
            Surface = surface.Surface,
            
            MinImageCount = desiredNumImages,
            PresentMode = desiredPresentMode,
            
            ImageFormat = desiredFormat,
            ImageColorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr,
            
            ImageExtent = desiredExtent,
            
            
        }
    }
    
    public override void Dispose()
    {
        
    }
}