using Silk.NET.Vulkan;

namespace grabs.Vulkan;

internal static class VulkanUtils
{
    public static void Check(this Result result, string operation)
    {
        if (result != Result.Success)
            throw new VulkanOperationException(operation, result);
    }
}