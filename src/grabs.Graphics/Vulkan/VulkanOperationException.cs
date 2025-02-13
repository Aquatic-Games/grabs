using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

public class VulkanOperationException : Exception
{
    public readonly string Operation;
    
    public readonly Result Result;

    public VulkanOperationException(string operation, Result result) : base(
        $"Vulkan operation '{operation}' failed with result: {result}")
    {
        Operation = operation;
        Result = result;
    }
}