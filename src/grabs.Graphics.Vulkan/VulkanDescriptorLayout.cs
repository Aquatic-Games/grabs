using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VulkanDescriptorLayout : DescriptorLayout
{
    private readonly Vk _vk;
    private readonly VkDevice _device;

    public readonly DescriptorSetLayout Layout;
    
    public VulkanDescriptorLayout(Vk vk, VkDevice device, ref readonly DescriptorLayoutInfo info)
    {
        _vk = vk;
        _device = device;

        DescriptorSetLayoutBinding* bindings = stackalloc DescriptorSetLayoutBinding[info.Bindings.Length];
        for (int i = 0; i < info.Bindings.Length; i++)
        {
            ref readonly DescriptorBinding binding = ref info.Bindings[i];

            bindings[i] = new DescriptorSetLayoutBinding()
            {
                Binding = binding.Binding,
                DescriptorType = binding.Type.ToVk(),
                DescriptorCount = 1,
                StageFlags = binding.Stages.ToVk()
            };
        }

        DescriptorSetLayoutCreateInfo descriptorInfo = new()
        {
            SType = StructureType.DescriptorSetLayoutCreateInfo,
            BindingCount = (uint) info.Bindings.Length,
            PBindings = bindings
        };

        GrabsLog.Log("Creating descriptor layout.");
        _vk.CreateDescriptorSetLayout(device, &descriptorInfo, null, out Layout)
            .Check("Create descriptor layout");
    }
    
    public override void Dispose()
    {
        _vk.DestroyDescriptorSetLayout(_device, Layout, null);
    }
}