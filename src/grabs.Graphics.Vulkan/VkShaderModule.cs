global using VulkanShaderModule = Silk.NET.Vulkan.ShaderModule;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkShaderModule : ShaderModule
{
    public override bool IsDisposed { get; protected set; }

    private readonly Vk _vk;
    private readonly VulkanDevice _device;
    
    public readonly VulkanShaderModule Module;
    public readonly string EntryPoint;

    public VkShaderModule(Vk vk, VulkanDevice device, byte[] spirv, string entryPoint)
    {
        ResourceTracker.RegisterDeviceResource(device, this);
        
        _vk = vk;
        _device = device;
        EntryPoint = entryPoint;

        fixed (byte* pSpirv = spirv)
        {
            ShaderModuleCreateInfo shaderModuleInfo = new()
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint) spirv.Length,
                PCode = (uint*) pSpirv
            };

            _vk.CreateShaderModule(_device, &shaderModuleInfo, null, out Module).Check("Create shader module");
        }
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        _vk.DestroyShaderModule(_device, Module, null);
        
        ResourceTracker.DeregisterDeviceResource(_device, this);
    }
}