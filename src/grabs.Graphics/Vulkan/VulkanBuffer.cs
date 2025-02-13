global using VkBuffer = Silk.NET.Vulkan.Buffer;
using grabs.VulkanMemoryAllocator;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanBuffer : Buffer
{
    private readonly Vk _vk;
    private readonly VmaAllocator_T* _allocator;

    private readonly VmaAllocation_T* _allocation;
    
    public readonly VkBuffer Buffer;

    public VulkanBuffer(Vk vk, VmaAllocator_T* allocator, ref readonly BufferInfo info)
    {
        _vk = vk;
        _allocator = allocator;

        BufferUsageFlags usage = info.Type switch
        {
            BufferType.Vertex => BufferUsageFlags.VertexBufferBit,
            BufferType.Index => BufferUsageFlags.IndexBufferBit,
            _ => throw new ArgumentOutOfRangeException()
        };

        BufferCreateInfo bufferInfo = new BufferCreateInfo()
        {
            SType = StructureType.BufferCreateInfo,
            Size = info.Size,
            Usage = usage
        };

        VmaAllocationCreateInfo allocInfo = new VmaAllocationCreateInfo()
        {
            usage = VmaMemoryUsage.VMA_MEMORY_USAGE_AUTO
        };

        Vma.CreateBuffer(_allocator, &bufferInfo, &allocInfo, out Buffer, out _allocation, out _).Check("Create buffer");
    }
    
    public override void Dispose()
    {
        Vma.DestroyBuffer(_allocator, Buffer, _allocation);
    }
}