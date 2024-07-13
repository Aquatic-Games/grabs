global using VulkanInstance = Silk.NET.Vulkan.Instance;

using System;
using System.Reflection;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

public unsafe class VkInstance : Instance
{
    public readonly Vk Vk;

    public VulkanInstance Instance;
    
    public override GraphicsApi Api => GraphicsApi.Vulkan;

    public VkInstance(string[] extensions, string appName = null, string engineName = null, Version appVersion = null, Version engineVersion = null)
    {
        Vk = Vk.GetApi();
        
        appName ??= Assembly.GetEntryAssembly()?.GetName().Name ?? "GRABS";
        engineName ??= "GRABS";
        uint vkAppVersion = appVersion != null
            ? Vk.MakeVersion((uint) appVersion.Major, (uint) appVersion.Major, (uint) appVersion.Build)
            : Vk.Version10;
        uint vkEngineVersion = engineVersion != null
            ? Vk.MakeVersion((uint) engineVersion.Major, (uint) engineVersion.Major, (uint) engineVersion.Build)
            : Vk.Version10;

        using PinnedString pAppName = new PinnedString(appName);
        using PinnedString pEngineName = new PinnedString(engineName);

        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            // TODO: Support Vulkan 1.0/1.1?
            ApiVersion = Vk.Version13,

            PApplicationName = pAppName,
            ApplicationVersion = vkAppVersion,
            
            PEngineName = pEngineName,
            EngineVersion = vkEngineVersion
        };

        using PinnedStringArray pExtensions = new PinnedStringArray(extensions);

        InstanceCreateInfo instanceCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
            EnabledExtensionCount = pExtensions.Length,
            PpEnabledExtensionNames = pExtensions
        };

        GrabsLog.Log(GrabsLog.LogType.Verbose, "Creating instance.");
        VkUtils.CheckResult(Vk.CreateInstance(&instanceCreateInfo, null, out Instance), "create instance");
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        Vk.DestroyInstance(Instance, null);
    }
}