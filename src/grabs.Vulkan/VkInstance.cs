using Silk.NET.Vulkan;

namespace grabs.Vulkan;

public class VkInstance : Instance
{
    public Vk Vk;

    public Silk.NET.Vulkan.Instance Instance;

    public unsafe VkInstance(string[] instanceExtensions)
    {
        Vk = Vk.GetApi();
        
        // TODO: Probably don't need to use Vulkan 1.3. Just using it right now so dynamic rendering can be used without extensions.
        // TODO: Support both dynamic rendering and render passes.
        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version13
        };

        using PinnedStringArray pInstanceExtensions = new PinnedStringArray(instanceExtensions);
        
        InstanceCreateInfo iCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            EnabledExtensionCount = (uint) pInstanceExtensions.Length,
            PpEnabledExtensionNames = (byte**) pInstanceExtensions.Handle
        };

        Result result;
        if ((result = Vk.CreateInstance(&iCreateInfo, null, out Instance)) != Result.Success)
            throw new Exception($"Failed to create instance: {result}");
    }
    
    public override Device CreateDevice(Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }

    public override unsafe void Dispose()
    {
        Vk.DestroyInstance(Instance, null);
        Vk.Dispose();
    }
}