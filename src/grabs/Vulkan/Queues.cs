using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal struct Queues
{
    public uint GraphicsIndex;
    public Queue Graphics;
    
    public uint PresentIndex;
    public Queue Present;

    public HashSet<uint> UniqueQueues => [GraphicsIndex, PresentIndex];
}