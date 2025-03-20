global using VkBuffer = Silk.NET.Vulkan.Buffer;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using grabs.Core;
using grabs.VulkanMemoryAllocator;
using Silk.NET.Vulkan;
using static grabs.VulkanMemoryAllocator.VmaAllocationCreateFlagBits;
using static grabs.VulkanMemoryAllocator.VmaMemoryUsage;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanBuffer : Buffer
{
    private readonly VmaAllocator_T* _allocator;
    private readonly VmaAllocation_T* _allocation;
    
    public readonly VkBuffer Buffer;

    public readonly bool IsPersistentMapped;
    public readonly void* MappedPtr;

    public readonly uint BufferSize;
    public ulong WriteOffset;
    public ulong ReadOffset;

    public VulkanBuffer(VulkanDevice device, ref readonly BufferInfo info, void* data) : base(in info)
    {
        _allocator = device.Allocator;
        BufferSize = info.Size;
        WriteOffset = 0;
        ReadOffset = 0;

        BufferUsageFlags usage = 0;
        VmaMemoryUsage bufferUsage = VMA_MEMORY_USAGE_AUTO;
        VmaAllocationCreateFlagBits bufferFlags = 0;

        bool isBufferType = false;

        if (info.Usage.HasFlag(BufferUsage.Vertex))
        {
            usage |= BufferUsageFlags.VertexBufferBit;
            isBufferType = true;
        }

        if (info.Usage.HasFlag(BufferUsage.Index))
        {
            usage |= BufferUsageFlags.IndexBufferBit;
            isBufferType = true;
        }

        if (info.Usage.HasFlag(BufferUsage.Constant))
        {
            usage |= BufferUsageFlags.UniformBufferBit;
            isBufferType = true;
        }

        if (info.Usage.HasFlag(BufferUsage.TransferDst))
            usage |= BufferUsageFlags.TransferDstBit;

        if (info.Usage.HasFlag(BufferUsage.TransferSrc))
        {
            usage |= BufferUsageFlags.TransferSrcBit;
            bufferFlags |= VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT | VMA_ALLOCATION_CREATE_MAPPED_BIT;
            IsPersistentMapped = true;
        }

        if (info.Usage.HasFlag(BufferUsage.MapWrite))
        {
            bufferFlags |= VMA_ALLOCATION_CREATE_HOST_ACCESS_SEQUENTIAL_WRITE_BIT |
                           VMA_ALLOCATION_CREATE_HOST_ACCESS_ALLOW_TRANSFER_INSTEAD_BIT |
                           VMA_ALLOCATION_CREATE_MAPPED_BIT;
            IsPersistentMapped = true;
        }

        if (info.Usage.HasFlag(BufferUsage.UpdateBuffer))
        {
            BufferSize *= VulkanBackend.RingBufferSizeMultiplier;
        }

        // Automatically enable transfer if we provide initial data.
        if (data != null && isBufferType)
            usage |= BufferUsageFlags.TransferDstBit;

        BufferCreateInfo bufferInfo = new BufferCreateInfo()
        {
            SType = StructureType.BufferCreateInfo,
            Size = BufferSize,
            Usage = usage
        };

        VmaAllocationCreateInfo allocInfo = new VmaAllocationCreateInfo()
        {
            usage = bufferUsage,
            flags = (uint) bufferFlags
        };

        GrabsLog.Log("Creating buffer.");
        Vma.CreateBuffer(_allocator, &bufferInfo, &allocInfo, out Buffer, out _allocation, out VmaAllocationInfo allocationInfo)
            .Check("Create buffer");

        if (IsPersistentMapped)
            MappedPtr = allocationInfo.pMappedData;

        if (data == null)
            return;
        
        if (isBufferType)
            device.UpdateBuffer(this, 0, info.Size, data);
        else
            throw new NotImplementedException();
    }

    public void Update(CommandBuffer cb, uint size, void* pData)
    {
        Debug.Assert(Info.Usage.HasFlag(BufferUsage.UpdateBuffer));
        Debug.Assert(IsPersistentMapped);

        if (WriteOffset + size >= BufferSize)
            WriteOffset = 0;

        ReadOffset = WriteOffset;
        
        Unsafe.CopyBlock((byte*) MappedPtr + WriteOffset, pData, size);

        WriteOffset += size;
    }
    
    protected override MappedData Map()
    {
        if (IsPersistentMapped)
            return new MappedData((nint) MappedPtr);
        
        void* pData;
        Vma.MapMemory(_allocator, _allocation, &pData).Check("Map buffer");

        return new MappedData((nint) pData);
    }

    protected override void Unmap()
    {
        if (IsPersistentMapped)
            return;
        
        Vma.UnmapMemory(_allocator, _allocation);
    }

    public override void Dispose()
    {
        Vma.DestroyBuffer(_allocator, Buffer, _allocation);
    }
}