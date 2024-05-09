using System;
using System.Drawing;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.Vulkan;
using grabs.Tests;
using grabs.Tests.Tests;
using Silk.NET.SDL;

GrabsLog.LogMessage += (type, message) => Console.WriteLine($"[{type}] {message}"); 

//using TestBase test = new CubeTest();
//test.Run(GraphicsApi.OpenGL, new Size(1280, 720));

unsafe
{
    Sdl sdl = Sdl.GetApi();

    if (sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
        throw new Exception("Failed to initialize SDL.");

    const uint width = 1280;
    const uint height = 720;

    Window* window = sdl.CreateWindow("vulkan", Sdl.WindowposCentered, Sdl.WindowposCentered, (int) width, (int) height,
        (uint) WindowFlags.Vulkan);

    if (window == null)
        throw new Exception("Failed to create SDL window.");

    uint numInstanceExtensions;
    sdl.VulkanGetInstanceExtensions(window, &numInstanceExtensions, (byte**) null);
    string[] extensions = new string[numInstanceExtensions];
    sdl.VulkanGetInstanceExtensions(window, &numInstanceExtensions, extensions);

    Instance instance = new VkInstance(extensions, true);
    
    instance.Dispose();
}