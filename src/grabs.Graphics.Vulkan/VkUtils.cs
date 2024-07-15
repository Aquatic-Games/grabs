using System;
using Silk.NET.Vulkan;

namespace grabs.Graphics.Vulkan;

public static class VkUtils
{
    public static void CheckResult(Result result, string operation)
    {
        if (result != Result.Success)
            throw new Exception($"Failed to {operation}: {result}");
    }

    public static void CheckResult(Result result)
    {
        if (result != Result.Success)
            throw new Exception($"Vulkan operation failed: {result}");
    }
}