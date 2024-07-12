using System;
using System.Drawing;
using grabs.Core;
using grabs.Graphics;
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

using TestBase test = new CubemapTest();
test.Run(api, new Size(1280, 720));