using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkSwapchain : Swapchain
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VkDevice _device;
    private readonly Fence _fence;

    private SwapchainKHR _swapchain;
    private VkTexture[] _swapchainTextures;
    private uint _currentImage;
    
    public override Format BufferFormat { get; }

    public VkSwapchain(Vk vk, VkDevice device, ref readonly SwapchainInfo info)
    {
        ResourceTracker.RegisterDeviceResource(device.Device, this);

        _vk = vk;
        _device = device;

        KhrSwapchain khrSwapchain = device.KhrSwapchain;
        KhrSurface khrSurface = device.KhrSurface;
        SurfaceKHR surface = ((VkSurface) info.Surface).Surface;

        SurfaceCapabilitiesKHR surfaceCapabilities;
        khrSurface.GetPhysicalDeviceSurfaceCapabilities(device.PhysicalDevice, surface, &surfaceCapabilities)
            .Check("Get surface capabilities");
        
        GrabsLog.Log($"Requested image count: {info.NumBuffers}");
        GrabsLog.Log($"Min image count: {surfaceCapabilities.MinImageCount}");
        GrabsLog.Log($"Max image count: {surfaceCapabilities.MaxImageCount}");

        uint numImages = uint.Clamp(info.NumBuffers, surfaceCapabilities.MinImageCount,
            surfaceCapabilities.MaxImageCount == 0 ? uint.MaxValue : surfaceCapabilities.MaxImageCount);
        GrabsLog.Log($"Image count: {numImages}");

        Extent2D extent = info.Size.ToVk();
        GrabsLog.Log($"Requested extent: {extent.Width}x{extent.Height}");
        GrabsLog.Log($"Min extent: {surfaceCapabilities.MinImageExtent.Width}x{surfaceCapabilities.MinImageExtent.Height}");
        GrabsLog.Log($"Max extent: {surfaceCapabilities.MaxImageExtent.Width}x{surfaceCapabilities.MaxImageExtent.Height}");

        extent.Width = uint.Clamp(extent.Width, surfaceCapabilities.MinImageExtent.Width,
            surfaceCapabilities.MaxImageExtent.Width);
        extent.Height = uint.Clamp(extent.Height, surfaceCapabilities.MinImageExtent.Height,
            surfaceCapabilities.MaxImageExtent.Height);
        GrabsLog.Log($"Image extent: {extent.Width}x{extent.Height}");

        PresentModeKHR presentMode = info.PresentMode.ToVk();
        GrabsLog.Log($"Requested present mode: {presentMode}");
        
        GrabsLog.Log("Checking if present mode is supported.");
        uint numPresentModes;
        khrSurface.GetPhysicalDeviceSurfacePresentModes(device.PhysicalDevice, surface, &numPresentModes, null);
        PresentModeKHR* presentModes = stackalloc PresentModeKHR[(int) numPresentModes];
        khrSurface.GetPhysicalDeviceSurfacePresentModes(device.PhysicalDevice, surface, &numPresentModes, presentModes);

        for (uint i = 0; i < numPresentModes; i++)
        {
            if (presentModes[i] == presentMode)
                goto PRESENT_MODE_SUPPORTED;
        }

        GrabsLog.Log("Present mode not supported! Picking appropriate substitute present mode.");
        presentMode = presentModes[0];
        
        PRESENT_MODE_SUPPORTED: ;
        
        GrabsLog.Log($"Present mode: {presentMode}");

        VulkanFormat format = info.Format.ToVk();
        GrabsLog.Log($"Requested surface format: {format}");
        
        GrabsLog.Log("Checking if format is supported.");
        uint numFormats;
        khrSurface.GetPhysicalDeviceSurfaceFormats(device.PhysicalDevice, surface, &numFormats, null);
        SurfaceFormatKHR* formats = stackalloc SurfaceFormatKHR[(int) numFormats];
        khrSurface.GetPhysicalDeviceSurfaceFormats(device.PhysicalDevice, surface, &numFormats, formats);

        for (uint i = 0; i < numFormats; i++)
        {
            if (formats[i].ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr && formats[i].Format == format)
                goto FORMAT_SUPPORTED;
        }

        throw new NotSupportedException($"Format {info.Format} not supported by the surface!");
        
        FORMAT_SUPPORTED: ;

        BufferFormat = info.Format;

        SwapchainCreateInfoKHR swapchainInfo = new()
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = surface,

            MinImageCount = numImages,
            ImageExtent = extent,
            ImageFormat = format,
            ImageArrayLayers = 1,
            ImageColorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,

            PresentMode = presentMode,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            Clipped = true,
            PreTransform = surfaceCapabilities.CurrentTransform
        };

        if (device.Queues.GraphicsIndex == device.Queues.PresentIndex)
            swapchainInfo.ImageSharingMode = SharingMode.Exclusive;
        else
            throw new NotImplementedException("Present and graphics queue are separate which is not yet supported.");

        GrabsLog.Log("Creating swapchain");
        khrSwapchain.CreateSwapchain(device.Device, &swapchainInfo, null, out _swapchain).Check("Create swapchain");
        
        GrabsLog.Log("Getting swapchain images.");
        uint numSwapchainImages;
        khrSwapchain.GetSwapchainImages(device.Device, _swapchain, &numSwapchainImages, null)
            .Check("Get swapchain images");
        Image* swapchainImages = stackalloc Image[(int) numSwapchainImages];
        khrSwapchain.GetSwapchainImages(device.Device, _swapchain, &numSwapchainImages, swapchainImages)
            .Check("Get swapchain images");

        _swapchainTextures = new VkTexture[numSwapchainImages];

        for (uint i = 0; i < numSwapchainImages; i++)
            _swapchainTextures[i] = new VkTexture(_vk, _device.Device, swapchainImages[i], format, new Size2D(extent.Width, extent.Height));

        FenceCreateInfo fenceInfo = new()
        {
            SType = StructureType.FenceCreateInfo
        };

        GrabsLog.Log("Creating fence.");
        _vk.CreateFence(_device.Device, &fenceInfo, null, out _fence).Check("Create fence");
    }

    public override Texture GetNextTexture()
    {
        // TODO: Check for invalid swapchains and recreate!
        _device.KhrSwapchain.AcquireNextImage(_device.Device, _swapchain, ulong.MaxValue, new Semaphore(), _fence,
            ref _currentImage).Check("Acquire next image");

        _vk.WaitForFences(_device.Device, 1, in _fence, true, ulong.MaxValue).Check("Wait for fence");
        _vk.ResetFences(_device.Device, 1, in _fence).Check("Reset fence");

        return _swapchainTextures[_currentImage];
    }

    public override void Present()
    {
        SwapchainKHR swapchain = _swapchain;
        uint currentImage = _currentImage;
        
        PresentInfoKHR presentInfo = new()
        {
            SType = StructureType.PresentInfoKhr,
            SwapchainCount = 1,
            PSwapchains = &swapchain,
            PImageIndices = &currentImage,
        };
        
        // TODO: Check for invalid swapchains and recreate!
        _device.KhrSwapchain.QueuePresent(_device.Queues.Present, &presentInfo).Check("Present");
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        GrabsLog.Log("Destroying swapchain textures.");
        foreach (VkTexture texture in _swapchainTextures)
            texture.Dispose();
        
        GrabsLog.Log("Destroying fence.");
        _vk.DestroyFence(_device.Device, _fence, null);
        
        GrabsLog.Log("Destroying swapchain.");
        _device.KhrSwapchain.DestroySwapchain(_device.Device, _swapchain, null);
        
        ResourceTracker.DeregisterDeviceResource(_device.Device, this);
    }
}