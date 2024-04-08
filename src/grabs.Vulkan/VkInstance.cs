using Silk.NET.Vulkan;

namespace grabs.Vulkan;

public class VkInstance : Instance
{
    public static Vk Vk;

    public Silk.NET.Vulkan.Instance Instance;

    static VkInstance()
    {
        Vk = Vk.GetApi();
    }

    public unsafe VkInstance()
    {
        // TODO: Probably don't need to use Vulkan 1.3. Just using it right now so dynamic rendering can be used without extensions.
        // TODO: Support both dynamic rendering and render passes.
        ApplicationInfo appInfo = new ApplicationInfo()
        {
            SType = StructureType.ApplicationInfo,
            ApiVersion = Vk.Version13
        };
        
        InstanceCreateInfo iCreateInfo = new InstanceCreateInfo()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            
        }
    }
    
    public override Device CreateDevice(Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}