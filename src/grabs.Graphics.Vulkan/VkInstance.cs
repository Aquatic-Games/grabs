using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

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

        List<string> instanceExtensions = [KhrSurface.ExtensionName];

        uint numInstanceExtensions;
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numInstanceExtensions, null);
        ExtensionProperties* extensionProperties = stackalloc ExtensionProperties[(int) numInstanceExtensions];
        _vk.EnumerateInstanceExtensionProperties((byte*) null, &numInstanceExtensions, extensionProperties);

        GrabsLog.Log("Available instance extensions:");
        
        for (uint i = 0; i < numInstanceExtensions; i++)
        {
            string extension = new string((sbyte*) extensionProperties[i].ExtensionName);
            GrabsLog.Log($"    {extension}");

            if (extension
                is KhrWin32Surface.ExtensionName
                or KhrXlibSurface.ExtensionName
                or KhrXcbSurface.ExtensionName
                or KhrWaylandSurface.ExtensionName)
            {
                instanceExtensions.Add(extension);
            }
        }

        GrabsLog.Log(GrabsLog.Severity.Debug,
            $"Enabled instance extensions: [{string.Join(", ", instanceExtensions)}]");

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
        
        _vk.Dispose();
    }
}