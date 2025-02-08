global using VkShaderModule = Silk.NET.Vulkan.ShaderModule;
using grabs.Core;
using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanShaderModule : ShaderModule
{
    private readonly Vk _vk;
    private readonly VkDevice _device;
    
    public readonly VkShaderModule Module;

    public readonly string EntryPoint;

    public VulkanShaderModule(Vk vk, VkDevice device, ShaderStage stage, ref readonly ReadOnlySpan<byte> spirv, string entryPoint)
    {
        _vk = vk;
        _device = device;

        EntryPoint = entryPoint;

        fixed (byte* pSpirv = spirv)
        {
            ShaderModuleCreateInfo moduleInfo = new ShaderModuleCreateInfo()
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint) spirv.Length,
                PCode = (uint*) pSpirv
            };
            
            GrabsLog.Log("Creating shader module");
            _vk.CreateShaderModule(_device, &moduleInfo, null, out Module).Check("Create shader module");
        }
    }
    
    public override void Dispose()
    {
        _vk.DestroyShaderModule(_device, Module, null);
    }
}