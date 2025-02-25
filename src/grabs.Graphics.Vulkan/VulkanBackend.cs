namespace grabs.Graphics.Vulkan;

public class VulkanBackend : IBackend
{
    public static string Name => "Vulkan";
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        throw new NotImplementedException();
    }
}