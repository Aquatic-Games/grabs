using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkInstance : Instance
{
    private readonly Vk _vk;
    
    public override bool IsDisposed { get; protected set; }

    public override string BackendName => VulkanBackend.Name;

    public VkInstance(ref readonly InstanceInfo info)
    {
        _vk = Vk.GetApi();

        using Utf8String pAppName = info.AppName;
        using Utf8String pEngineName = "GRABS";

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = pAppName,
            ApplicationVersion = Vk.MakeVersion(1, 0),
            PEngineName = pEngineName,
            EngineVersion = Vk.MakeVersion(1, 0),
            ApiVersion = Vk.Version13
        };

        InstanceCreateInfo instanceInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo
        };
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;
    }
}