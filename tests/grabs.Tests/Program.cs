using System;
using System.Drawing;
using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.Vulkan;
using grabs.Tests;
using grabs.Tests.Tests;
using Silk.NET.SDL;

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

using TestBase test = new BasicTest();
test.Run(api, new Size(1280, 720));

return;

unsafe
{
    Sdl sdl = Sdl.GetApi();

    if (sdl.Init(Sdl.InitVideo | Sdl.InitAudio) < 0)
        throw new Exception($"Failed to initialize SDL: {sdl.GetErrorS()}");

    const int width = 1280;
    const int height = 720;

    Window* window = sdl.CreateWindow("Vulkan Test", Sdl.WindowposCentered, Sdl.WindowposCentered, width, height,
        (uint) WindowFlags.Vulkan);

    if (window == null)
        throw new Exception($"Failed to create window: {sdl.GetErrorS()}");

    uint numExtensions;
    sdl.VulkanGetInstanceExtensions(window, &numExtensions, (byte**) null);
    string[] extensions = new string[numExtensions];
    sdl.VulkanGetInstanceExtensions(window, &numExtensions, extensions);

    Instance instance = new VkInstance(extensions);
    Adapter[] adapters = instance.EnumerateAdapters();
    Console.WriteLine($"Adapters:\n{string.Join("\n", adapters)}");

    bool alive = true;
    while (alive)
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
                            alive = false;
                            break;
                    }

                    break;
                }
            }
        }
    }
    
    instance.Dispose();
    
    sdl.DestroyWindow(window);
    sdl.Quit();
    sdl.Dispose();
}