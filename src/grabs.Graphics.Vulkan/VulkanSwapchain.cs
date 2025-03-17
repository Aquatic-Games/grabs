using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly KhrSwapchain _khrSwapchain;
    private readonly VulkanDevice _device;
    private readonly SurfaceKHR _surface;

    private readonly uint _numBuffers;
    private readonly PresentMode _presentMode;

    private VulkanTexture[] _swapchainTextures;
    private uint _currentImage;

    private readonly Fence _fence;

    private readonly CommandBuffer _commandBuffer;
    
    public SwapchainKHR Swapchain;
    
    public override Format SwapchainFormat { get; }
    
    public override Size2D Size { get; }

    public VulkanSwapchain(Vk vk, VulkanDevice device, ref readonly SwapchainInfo info)
    {
        _vk = vk;
        _device = device;
        _khrSwapchain = _device.KhrSwapchain;
        _surface = ((VulkanSurface) info.Surface).Surface;

        _numBuffers = info.NumBuffers;
        _presentMode = info.PresentMode;

        _swapchainTextures = [];

        Size = info.Size;
        SwapchainFormat = info.Format;

        CreateSwapchain();

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

    public override Texture GetNextTexture()
    {
        Result result = _khrSwapchain.AcquireNextImage(_device.Device, Swapchain, ulong.MaxValue, new Semaphore(), _fence, ref _currentImage);

        if (result is Result.SuboptimalKhr or Result.ErrorOutOfDateKhr)
            RecreateSwapchain();
        else
            result.Check("Acquire next image");
        
        _vk.WaitForFences(_device.Device, 1, in _fence, true, ulong.MaxValue)
            .Check("Wait for fence");
        _vk.ResetFences(_device.Device, 1, in _fence);

        CommandBufferBeginInfo beginInfo = new CommandBufferBeginInfo()
        {
            SType = StructureType.CommandBufferBeginInfo,
        };

        _vk.BeginCommandBuffer(_commandBuffer, &beginInfo).Check("Begin command buffer");

        VulkanUtils.ImageBarrier(_vk, _commandBuffer, _swapchainTextures[_currentImage].Image, ImageLayout.Undefined,
            ImageLayout.ColorAttachmentOptimal);
        
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

        return _swapchainTextures[_currentImage];
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

        VulkanUtils.ImageBarrier(_vk, _commandBuffer, _swapchainTextures[currentImage].Image,
            ImageLayout.ColorAttachmentOptimal, ImageLayout.PresentSrcKhr);
        
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

        PresentInfoKHR presentInfo = new PresentInfoKHR()
        {
            SType = StructureType.PresentInfoKhr,

            SwapchainCount = 1,
            PSwapchains = &swapchain,

            PImageIndices = &currentImage,
        };

        Result result = _khrSwapchain.QueuePresent(_device.Queues.Present, &presentInfo);
        
        if (result is Result.SuboptimalKhr or Result.ErrorOutOfDateKhr)
            RecreateSwapchain();
        else
            result.Check("Present");
    }
    
    private void CreateSwapchain()
    {
        PhysicalDevice physicalDevice = _device.PhysicalDevice;
        KhrSurface khrSurface = _device.KhrSurface;
        
        SurfaceCapabilitiesKHR surfaceCapabilities;
        khrSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, _surface, &surfaceCapabilities);

        uint desiredNumImages = _numBuffers;
        if (desiredNumImages < surfaceCapabilities.MinImageCount)
        {
            GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Type.General,
                $"Desired buffer count ({desiredNumImages}) is under the minimum allowed buffer count ({surfaceCapabilities.MinImageCount}). Clamping to the minimum.");

            desiredNumImages = surfaceCapabilities.MinImageCount;
        }
        // If max image count is 0, there is no limit.
        else if (desiredNumImages > surfaceCapabilities.MaxImageCount && surfaceCapabilities.MaxImageCount != 0)
        {
            GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Type.General,
                $"Desired buffer count ({desiredNumImages}) is above the maximum allowed buffer count ({surfaceCapabilities.MaxImageCount}). Clamping to the maximum.");

            desiredNumImages = surfaceCapabilities.MaxImageCount;
        }
        
        GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Type.General,
            "Checking supported present modes over selected present mode.");
        
        uint numPresentModes;
        khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, _surface, &numPresentModes, null);
        PresentModeKHR* presentModes = stackalloc PresentModeKHR[(int) numPresentModes];
        khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, _surface, &numPresentModes, presentModes);

        PresentModeKHR desiredPresentMode = _presentMode switch
        {
            PresentMode.Immediate => PresentModeKHR.ImmediateKhr,
            PresentMode.Fifo => PresentModeKHR.FifoKhr,
            PresentMode.FifoRelaxed => PresentModeKHR.FifoRelaxedKhr,
            PresentMode.Mailbox => PresentModeKHR.MailboxKhr,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        // TODO: Make this smarter so FifoRelaxed will prefer Fifo if not present, etc.
        for (uint i = 0; i < numPresentModes; i++)
        {
            if (desiredPresentMode == presentModes[i])
                goto PRESENT_MODE_FOUND;
        }

        GrabsLog.Log(GrabsLog.Severity.Warning, GrabsLog.Type.Performance,
            $"Desired present mode {desiredPresentMode} is not supported by the GPU - present mode {presentModes[0]} will be used instead.");

        desiredPresentMode = presentModes[0];
        
        PRESENT_MODE_FOUND: ;
        
        VkFormat format = SwapchainFormat.ToVk();

        // TODO: Compare desiredExtent vs the Min and Max extents.
        Extent2D desiredExtent = Size.ToVk();

        SwapchainCreateInfoKHR swapchainInfo = new SwapchainCreateInfoKHR()
        {
            SType = StructureType.SwapchainCreateInfoKhr,

            Surface = _surface,

            MinImageCount = desiredNumImages,
            PresentMode = desiredPresentMode,

            ImageFormat = format,
            ImageColorSpace = ColorSpaceKHR.SpaceSrgbNonlinearKhr,

            ImageExtent = desiredExtent,

            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            ImageArrayLayers = 1,

            Clipped = true,
            CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
            PreTransform = SurfaceTransformFlagsKHR.IdentityBitKhr,

            Flags = SwapchainCreateFlagsKHR.None,
            OldSwapchain = Swapchain
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
        Image* swapchainImages = stackalloc Image[(int) numImages];
        _khrSwapchain.GetSwapchainImages(_device.Device, Swapchain, &numImages, swapchainImages);

        GrabsLog.Log("Creating swapchain image views.");
        _swapchainTextures = new VulkanTexture[numImages];
        for (int i = 0; i < numImages; i++)
        {
            ImageViewCreateInfo imageViewInfo = new ImageViewCreateInfo()
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = swapchainImages[i],

                Format = format,
                ViewType = ImageViewType.Type2D,
                Components = new ComponentMapping(ComponentSwizzle.Identity, ComponentSwizzle.Identity, ComponentSwizzle.Identity, ComponentSwizzle.Identity),

                SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1)
            };
            
            _vk.CreateImageView(_device.Device, &imageViewInfo, null, out ImageView view)
                .Check("Create image view");

            _swapchainTextures[i] = new VulkanTexture(_vk, _device.Device, swapchainImages[i], view, Size);
        }
    }

    private void RecreateSwapchain()
    {
        foreach (VulkanTexture texture in _swapchainTextures)
            texture.Dispose();

        SwapchainKHR oldChain = Swapchain;
        CreateSwapchain();
        _khrSwapchain.DestroySwapchain(_device.Device, oldChain, null);
    }

    public override void Dispose()
    {
        _vk.FreeCommandBuffers(_device.Device, _device.CommandPool, 1, in _commandBuffer);
        
        _vk.DestroyFence(_device.Device, _fence, null);
        
        foreach (VulkanTexture texture in _swapchainTextures)
            texture.Dispose();
        
        _khrSwapchain.DestroySwapchain(_device.Device, Swapchain, null);
    }
}