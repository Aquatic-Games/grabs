using System;
using System.Drawing;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.Vulkan;
using grabs.Tests;
using grabs.Tests.Tests;
using Silk.NET.Core.Native;
using Silk.NET.SDL;
using Silk.NET.Vulkan;
using Device = grabs.Graphics.Device;
using Event = Silk.NET.SDL.Event;
using Instance = grabs.Graphics.Instance;
using Surface = grabs.Graphics.Surface;

GrabsLog.LogMessage += (type, message) => Console.WriteLine($"[{type}] {message}");

GraphicsApi api = 0;

unsafe
{
    using PinnedString title = new PinnedString("Choose API");
    using PinnedString message = new PinnedString("DirectX 11: Recommended for most systems.\nVulkan: Experimental.\nOpenGL: Legacy. Use if no other options work.");

    using PinnedString d3dButtonText = new PinnedString("DirectX 11");
    using PinnedString vkButtonText = new PinnedString("Vulkan");
    using PinnedString glButtonText = new PinnedString("OpenGL");
    using PinnedString closeButtonText = new PinnedString("Cancel");
    
    MessageBoxButtonData d3dButton = new MessageBoxButtonData()
    {
        Buttonid = (int) GraphicsApi.D3D11,
        Flags = (uint) MessageBoxButtonFlags.ReturnkeyDefault,
        Text = d3dButtonText
    };
    
    MessageBoxButtonData vkButton = new MessageBoxButtonData()
    {
        Buttonid = (int) GraphicsApi.Vulkan,
        Flags = 0,
        Text = vkButtonText
    };

    MessageBoxButtonData glButton = new MessageBoxButtonData()
    {
        Buttonid = (int) GraphicsApi.OpenGL,
        Flags = 0,
        Text = glButtonText
    };

    MessageBoxButtonData closeButton = new MessageBoxButtonData()
    {
        Buttonid = -1,
        Flags = (uint) MessageBoxButtonFlags.EscapekeyDefault,
        Text = closeButtonText
    };

    MessageBoxButtonData* buttons = stackalloc MessageBoxButtonData[]
    {
        d3dButton, vkButton, glButton, closeButton
    };

    MessageBoxData data = new MessageBoxData()
    {
        Flags = (uint) MessageBoxFlags.ButtonsLeftToRight,
        Window = null,
        Title = title,
        Message = message,

        Numbuttons = 4,
        Buttons = buttons
    };
    
    Sdl sdl = Sdl.GetApi();
    int buttonId;
    sdl.ShowMessageBox(&data, &buttonId);

    if (buttonId == -1)
        return;
    
    api = (GraphicsApi) buttonId;
}

using TestBase test = new CubemapTest();
test.Run(api, new Size(1280, 720));

/*unsafe
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

    Adapter[] adapters = instance.EnumerateAdapters();
    Console.WriteLine(string.Join("\n", adapters));

    VkNonDispatchableHandle surfaceHandle;
    sdl.VulkanCreateSurface(window, new VkHandle(((VkInstance) instance).Instance.Handle), &surfaceHandle);

    Surface surface = new VkSurface(new SurfaceKHR(surfaceHandle.Handle), (VkInstance) instance);
    Device device = instance.CreateDevice(surface);

    Swapchain swapchain = device.CreateSwapchain(new SwapchainDescription(width, height));

    bool isAlive = true;
    while (isAlive)
    {
        Event winEvent;
        while (sdl.PollEvent(&winEvent) != 0)
        {
            switch ((EventType) winEvent.Type)
            {
                case EventType.Windowevent:
                {
                    switch ((WindowEventID) winEvent.Window.Event)
                    {
                        case WindowEventID.Close:
                            isAlive = false;
                            break;
                    }

                    break;
                }
            }
        }
    }
    
    swapchain.Dispose();
    device.Dispose();
    surface.Dispose();
    instance.Dispose();
}*/