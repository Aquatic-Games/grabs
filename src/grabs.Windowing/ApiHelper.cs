using System;
using grabs.Graphics;

namespace grabs.Windowing;

public static class ApiHelper
{
    public static bool IsGraphicsApiSupported(GraphicsApi api)
    {
        return api switch
        {
            GraphicsApi.D3D11 => OperatingSystem.IsWindows(),
            GraphicsApi.OpenGL => OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD(),
            GraphicsApi.OpenGLES => !OperatingSystem.IsMacOS() && !OperatingSystem.IsMacOS(),
            GraphicsApi.Vulkan => false, // Vulkan is not yet supported
            _ => throw new ArgumentOutOfRangeException(nameof(api), api, null)
        };
    }

    public static GraphicsApi PickBestGraphicsApi()
    {
        if (IsGraphicsApiSupported(GraphicsApi.D3D11))
            return GraphicsApi.D3D11;

        if (IsGraphicsApiSupported(GraphicsApi.Vulkan))
            return GraphicsApi.Vulkan;

        if (IsGraphicsApiSupported(GraphicsApi.OpenGL))
            return GraphicsApi.OpenGL;

        if (IsGraphicsApiSupported(GraphicsApi.OpenGLES))
            return GraphicsApi.OpenGLES;

        throw new Exception("No graphics API is supported.");
    }
}