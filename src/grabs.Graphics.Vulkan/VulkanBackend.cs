namespace grabs.Graphics.Vulkan;

public class VulkanBackend : IBackend
{
    public static string Name => "Vulkan";

    public static uint RingBufferSizeMultiplier = 16;
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        return new VulkanInstance(in info);
    }
}