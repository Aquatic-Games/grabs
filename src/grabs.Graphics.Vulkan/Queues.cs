using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal struct Queues
{
    public uint GraphicsIndex;
    public Queue Graphics;
    
    public uint PresentIndex;
    public Queue Present;

    public HashSet<uint> UniqueQueues => [GraphicsIndex, PresentIndex];
}