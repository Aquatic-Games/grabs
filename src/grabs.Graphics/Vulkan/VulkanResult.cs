using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal static class VulkanResult
{
    public static void CheckResult(Result result, string operation)
    {
        if (result != Result.Success)
            throw new Exception($"Vulkan Operation '{operation}' failed with result: {result}");
    }
}