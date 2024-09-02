using System;
using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using static grabs.Graphics.Vulkan.VkResult;

namespace grabs.Graphics.Vulkan;

public sealed unsafe class VkSwapchain : Swapchain
{
    private readonly Vk _vk;
    private readonly VulkanDevice _device;

    private readonly Format _format;
    private uint _width;
    private uint _height;
    
    private readonly KhrSwapchain _khrSwapchain;
    
    private readonly Queue _graphicsQueue;
    private readonly Queue _presentQueue;

    private readonly Semaphore _presentSemaphore;
    private readonly Semaphore _renderSemaphore;

    private readonly Fence _waitFence;

    public readonly SwapchainKHR Swapchain;
    
    public override PresentMode PresentMode { get; set; }

    public VkSwapchain(Vk vk, VkDevice device, SurfaceKHR surface, in SwapchainDescription description)
    {
        _vk = vk;
        _device = device.Device;
        _khrSwapchain = device.KhrSwapchain;
        
        _graphicsQueue = device.GraphicsQueue;
        _presentQueue = device.PresentQueue;

        _format = description.Format;
        _width = description.Width;
        _height = description.Height;

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

        SemaphoreCreateInfo semaphoreInfo = new SemaphoreCreateInfo()
        {
            SType = StructureType.SemaphoreCreateInfo
        };
        
        CheckResult(_vk.CreateSemaphore(_device, &semaphoreInfo, null, out _presentSemaphore), "Create present semaphore");
        CheckResult(_vk.CreateSemaphore(_device, &semaphoreInfo, null, out _renderSemaphore), "Create render semaphore");

        FenceCreateInfo fenceInfo = new FenceCreateInfo()
        {
            SType = StructureType.FenceCreateInfo,
            Flags = FenceCreateFlags.SignaledBit
        };
        
        CheckResult(_vk.CreateFence(_device, &fenceInfo, null, out _waitFence), "Create wait fence");
    }
    
    public override Texture GetSwapchainTexture()
    {
        uint numImages;
        _khrSwapchain.GetSwapchainImages(_device, Swapchain, &numImages, null);
        Span<Image> images = stackalloc Image[(int) numImages];
        fixed (Image* pImages = images)
            _khrSwapchain.GetSwapchainImages(_device, Swapchain, &numImages, pImages);

        return new VkSwapchainTexture(_vk, _device, images, _format, _width, _height);
    }

    public override void Resize(uint width, uint height)
    {
        throw new System.NotImplementedException();
    }

    public override void Present()
    {
        _vk.WaitForFences(_device, 1, in _waitFence, true, ulong.MaxValue);
        _vk.ResetFences(_device, 1, in _waitFence);

        uint imageIndex;
        CheckResult(_khrSwapchain.AcquireNextImage(_device, Swapchain, ulong.MaxValue, _presentSemaphore, new Fence(), &imageIndex), "Acquire next image");

        PipelineStageFlags waitStage = PipelineStageFlags.ColorAttachmentOutputBit;

        Semaphore presentSemaphore = _presentSemaphore;
        Semaphore renderSemaphore = _renderSemaphore;
        SwapchainKHR swapchain = Swapchain;

        SubmitInfo submitInfo = new SubmitInfo()
        {
            SType = StructureType.SubmitInfo,
            PWaitDstStageMask = &waitStage,

            CommandBufferCount = 0,

            WaitSemaphoreCount = 1,
            PWaitSemaphores = &presentSemaphore,

            SignalSemaphoreCount = 1,
            PSignalSemaphores = &renderSemaphore
        };

        CheckResult(_vk.QueueSubmit(_graphicsQueue, 1, &submitInfo, _waitFence));

        PresentInfoKHR presentInfo = new PresentInfoKHR()
        {
            SType = StructureType.PresentInfoKhr,

            WaitSemaphoreCount = 1,
            PWaitSemaphores = &renderSemaphore,

            SwapchainCount = 1,
            PSwapchains = &swapchain,
            PImageIndices = &imageIndex
        };
        
        CheckResult(_khrSwapchain.QueuePresent(_presentQueue, &presentInfo), "Present");
    }

    public override void Dispose()
    {
        _vk.DestroyFence(_device, _waitFence, null);
        _vk.DestroySemaphore(_device, _renderSemaphore, null);
        _vk.DestroySemaphore(_device, _presentSemaphore, null);
        
        _khrSwapchain.DestroySwapchain(_device, Swapchain, null);
    }
}