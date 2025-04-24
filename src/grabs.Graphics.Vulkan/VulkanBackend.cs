namespace grabs.Graphics.Vulkan;

public sealed class VulkanBackend : IBackend
{
    /// <inheritdoc />
    public static string Name => "Vulkan";

    /// <inheritdoc />
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        throw new NotImplementedException();
    }
}