global using VulkanInstance = Silk.NET.Vulkan.Instance;

namespace grabs.Graphics.Vk;

internal sealed class VkInstance : Instance
{
    private readonly Silk.NET.Vulkan.Vk _vk;
    
    public readonly VulkanInstance Instance;
    
    public override bool IsDisposed { get; protected set; }

    public override Backend Backend => Backend.Vulkan;

    public VkInstance(bool debug, IWindowProvider provider)
    {
        _vk = Silk.NET.Vulkan.Vk.GetApi();
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}