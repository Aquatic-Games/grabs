global using VkBuffer = Silk.NET.Vulkan.Buffer;
using System.Runtime.CompilerServices;
using grabs.Core;
using grabs.VulkanMemoryAllocator;
using Silk.NET.Vulkan;
using static grabs.VulkanMemoryAllocator.VmaAllocationCreateFlagBits;
using static grabs.VulkanMemoryAllocator.VmaMemoryUsage;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanBuffer : Buffer
{
    private readonly Vk _vk;
    private readonly VmaAllocator_T* _allocator;

    private readonly VmaAllocation_T* _allocation;
    
    public readonly VkBuffer Buffer;

    public VulkanBuffer(Vk vk, VulkanDevice device, ref readonly BufferInfo info, void* data)
    {
        _vk = vk;
        _allocator = device.Allocator;

        BufferUsageFlags usage = info.Type switch
        {
            BufferType.Vertex => BufferUsageFlags.VertexBufferBit,
            BufferType.Index => BufferUsageFlags.IndexBufferBit,
            BufferType.Constant => BufferUsageFlags.UniformBufferBit,
            _ => throw new ArgumentOutOfRangeException()
        };

        usage |= BufferUsageFlags.TransferDstBit;

        BufferCreateInfo bufferInfo = new BufferCreateInfo()
        {
            SType = StructureType.BufferCreateInfo,
            Size = info.Size,
            Usage = usage
        };

        VmaMemoryUsage bufferUsage;
        VmaAllocationCreateFlagBits bufferFlags = 0;

        if (info.Dynamic)
        {
            bufferUsage = VMA_MEMORY_USAGE_AUTO;
            bufferFlags = VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT;
        }
        else
            bufferUsage = VMA_MEMORY_USAGE_AUTO_PREFER_DEVICE;

        VmaAllocationCreateInfo allocInfo = new VmaAllocationCreateInfo()
        {
            usage = bufferUsage,
            flags = (uint) bufferFlags
        };

        GrabsLog.Log("Creating buffer.");
        Vma.CreateBuffer(_allocator, &bufferInfo, &allocInfo, out Buffer, out _allocation, out _)
            .Check("Create buffer");

        if (data == null)
            return;

        if (info.Dynamic)
        {
            void* mappedData;
            Vma.MapMemory(_allocator, _allocation, &mappedData).Check("Map memory");
            Unsafe.CopyBlock(mappedData, data, info.Size);
            Vma.UnmapMemory(_allocator, _allocation);
        }
        else
        {
            BufferCreateInfo stagingInfo = new BufferCreateInfo()
            {
                SType = StructureType.BufferCreateInfo,
                Size = info.Size,
                Usage = BufferUsageFlags.TransferSrcBit,
                SharingMode = SharingMode.Exclusive
            };

            VmaAllocationCreateInfo stagingAllocInfo = new VmaAllocationCreateInfo()
            {
                usage = VMA_MEMORY_USAGE_AUTO,
                flags = (uint) (VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT |
                                VMA_ALLOCATION_CREATE_MAPPED_BIT),
            };

            GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Type.Performance,
                $"data was not null, creating staging buffer with size {info.Size}");
            Vma.CreateBuffer(_allocator, &stagingInfo, &stagingAllocInfo, out VkBuffer staging, out VmaAllocation_T* stagingAllocation, out VmaAllocationInfo stagingAllocationInfo)
                .Check("Create staging buffer");

            Unsafe.CopyBlock(stagingAllocationInfo.pMappedData, data, info.Size);

            CommandBuffer cb = device.BeginCommands();

            BufferCopy copy = new BufferCopy()
            {
                Size = info.Size
            };
            _vk.CmdCopyBuffer(cb, staging, Buffer, 1, &copy);

            device.EndCommands();

            Vma.DestroyBuffer(_allocator, staging, stagingAllocation);
        }
    }
    
    protected override MappedData Map(MapMode mode)
    {
        void* pData;
        Vma.MapMemory(_allocator, _allocation, &pData).Check("Map buffer");

        return new MappedData((nint) pData);
    }
    
    protected override void Unmap()
    {
        Vma.UnmapMemory(_allocator, _allocation);
    }
    
    public override void Dispose()
    {
        Vma.DestroyBuffer(_allocator, Buffer, _allocation);
    }
}