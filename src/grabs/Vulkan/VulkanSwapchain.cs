using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly KhrSwapchain _khrSwapchain;
    private readonly VulkanDevice _device;

    private readonly Image[] _swapchainImages;
    private readonly ImageView[] _imageViews;
    private uint _currentImage;

    private readonly Fence _fence;

    private readonly CommandBuffer _commandBuffer;
    
    public readonly SwapchainKHR Swapchain;

    public VulkanSwapchain(Vk vk, VulkanDevice device, VulkanSurface surface, ref readonly SwapchainInfo info)
    {
        _vk = vk;
        _device = device;
        _khrSwapchain = _device.KhrSwapchain;

        PhysicalDevice physicalDevice = _device.PhysicalDevice;
        KhrSurface khrSurface = _device.KhrSurface;
        
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
        khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface.Surface, &numPresentModes, presentModes);

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

            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageArrayLayers = 1,

            Clipped = true,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,

            Flags = SwapchainCreateFlagsKHR.None
        };

        if (_device.Queues.PresentIndex == _device.Queues.GraphicsIndex)
            swapchainInfo.ImageSharingMode = SharingMode.Exclusive;
        else
            throw new NotImplementedException("GRABS does not yet support separate graphics and present queues.");

        GrabsLog.Log("Creating swapchain.");
        _khrSwapchain.CreateSwapchain(_device.Device, &swapchainInfo, null, out Swapchain)
            .Check("Create swapchain");

        GrabsLog.Log("Getting swapchain images");
        uint numImages;
        _khrSwapchain.GetSwapchainImages(_device.Device, Swapchain, &numImages, null);
        _swapchainImages = new Image[numImages];
        fixed (Image* pImages = _swapchainImages)
            _khrSwapchain.GetSwapchainImages(_device.Device, Swapchain, &numImages, pImages);

        GrabsLog.Log("Creating swapchain image views.");
        _imageViews = new ImageView[numImages];
        for (int i = 0; i < numImages; i++)
        {
            ImageViewCreateInfo imageViewInfo = new ImageViewCreateInfo()
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = _swapchainImages[i],

                Format = desiredFormat,
                ViewType = ImageViewType.Type2D,
                Components = new ComponentMapping(ComponentSwizzle.R, ComponentSwizzle.G, ComponentSwizzle.B, ComponentSwizzle.A),

                SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1)
            };
            
            _vk.CreateImageView(_device.Device, &imageViewInfo, null, out _imageViews[i])
                .Check("Create image view");
        }

        FenceCreateInfo fenceInfo = new FenceCreateInfo()
        {
            SType = StructureType.FenceCreateInfo,
        };
        
        GrabsLog.Log("Creating fence");
        _vk.CreateFence(_device.Device, &fenceInfo, null, out _fence).Check("Create fence");

        CommandBufferAllocateInfo allocInfo = new CommandBufferAllocateInfo()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _device.CommandPool,
            CommandBufferCount = 1,
            Level = CommandBufferLevel.Primary,
        };

        _vk.AllocateCommandBuffers(_device.Device, &allocInfo, out _commandBuffer).Check("Allocate command buffer");
    }

    public override void GetNextTexture()
    {
        _khrSwapchain.AcquireNextImage(_device.Device, Swapchain, ulong.MaxValue, new Semaphore(), _fence, ref _currentImage)
            .Check("Acquire next image");
        
        _vk.WaitForFences(_device.Device, 1, in _fence, true, ulong.MaxValue)
            .Check("Wait for fence");
        _vk.ResetFences(_device.Device, 1, in _fence);

        CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo()
        {
            SType = StructureType.CommandBufferBeginInfo,
        };

        _vk.BeginCommandBuffer(_commandBuffer, &beginInfo).Check("Begin command buffer");

        ImageMemoryBarrier memoryBarrier = new ImageMemoryBarrier()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = _swapchainImages[_currentImage],
            OldLayout = ImageLayout.Undefined,
            NewLayout = ImageLayout.ColorAttachmentOptimal,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit,
            SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1)
        };

        _vk.CmdPipelineBarrier(_commandBuffer, PipelineStageFlags.TopOfPipeBit,
            PipelineStageFlags.ColorAttachmentOutputBit, 0, 0, null, 0, null, 1, &memoryBarrier);
        
        _vk.EndCommandBuffer(_commandBuffer).Check("End command buffer");

        CommandBuffer buffer = _commandBuffer;

        SubmitInfo submitInfo = new SubmitInfo()
        {
            SType = StructureType.SubmitInfo,

            CommandBufferCount = 1,
            PCommandBuffers = &buffer
        };
        
        _vk.QueueSubmit(_device.Queues.Graphics, 1, &submitInfo, _fence).Check("Submit queue");

        _vk.WaitForFences(_device.Device, 1, in _fence, true, ulong.MaxValue);
        _vk.ResetFences(_device.Device, 1, in _fence);
        
        Console.WriteLine("Got here!");
    }

    public override void Present()
    {
        SwapchainKHR swapchain = Swapchain;
        uint currentImage = _currentImage;
        
        CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo()
        {
            SType = StructureType.CommandBufferBeginInfo,
        };

        _vk.BeginCommandBuffer(_commandBuffer, &beginInfo).Check("Begin command buffer");

        ImageMemoryBarrier memoryBarrier = new ImageMemoryBarrier()
        {
            SType = StructureType.ImageMemoryBarrier,
            Image = _swapchainImages[currentImage],
            OldLayout = ImageLayout.ColorAttachmentOptimal,
            NewLayout = ImageLayout.PresentSrcKhr,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit,
            SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1)
        };

        _vk.CmdPipelineBarrier(_commandBuffer, PipelineStageFlags.ColorAttachmentOutputBit,
            PipelineStageFlags.BottomOfPipeBit, 0, 0, null, 0, null, 1, &memoryBarrier);
        
        _vk.EndCommandBuffer(_commandBuffer).Check("End command buffer");

        CommandBuffer buffer = _commandBuffer;

        SubmitInfo submitInfo = new SubmitInfo()
        {
            SType = StructureType.SubmitInfo,

            CommandBufferCount = 1,
            PCommandBuffers = &buffer
        };
        
        _vk.QueueSubmit(_device.Queues.Graphics, 1, &submitInfo, _fence).Check("Submit queue");

        _vk.WaitForFences(_device.Device, 1, in _fence, true, ulong.MaxValue);
        _vk.ResetFences(_device.Device, 1, in _fence);
        
        Console.WriteLine("Got here too");

        PresentInfoKHR presentInfo = new PresentInfoKHR()
        {
            SType = StructureType.PresentInfoKhr,

            SwapchainCount = 1,
            PSwapchains = &swapchain,

            PImageIndices = &currentImage,
        };
        
        _khrSwapchain.QueuePresent(_device.Queues.Present, &presentInfo).Check("Present");
    }

    public override void Dispose()
    {
        _khrSwapchain.DestroySwapchain(_device.Device, Swapchain, null);
    }
}