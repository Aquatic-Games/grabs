using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

internal static class VkUtils
{
    public static void Check(this Result result, string operation)
    {
        if (result != Result.Success)
            throw new Exception($"Vulkan operation '{operation}' failed: {result}");
    }
}