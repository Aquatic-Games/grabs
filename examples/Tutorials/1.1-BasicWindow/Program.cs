using System.Numerics;
using grabs.Graphics;
using grabs.Windowing;
using grabs.Windowing.Events;

/*
 * Basic windowing tutorial.
 * Creates a window, and renders a color to the screen.
 */

const uint width = 1280;
const uint height = 720;

// Create our window. There are some extra parameters in the constructor, but the default values are fine in this case.
WindowInfo windowInfo = new WindowInfo("Tutorial 1.1 - Basic Window", width, height);
using Window window = new Window(windowInfo);

// Create an instance and surface.
// An instance is used to create devices and enumerate adapters, and is the "jumping off" point for everything.
// A surface is the actual bit of the window that gets rendered to.
using Instance instance = window.CreateInstance();
using Surface surface = window.CreateSurface();

// Create our device and command list.
// A device represents a logical graphics device, inside the user's machine.
// A command list contains a list of graphics that are sent to the device to be executed.
using Device device = instance.CreateDevice(surface);
using CommandList commandList = device.CreateCommandList();

SwapchainDescription swapchainDesc = new SwapchainDescription(width, height, presentMode: PresentMode.VerticalSync);
using Swapchain swapchain = device.CreateSwapchain(swapchainDesc);
using Texture swapchainTexture = swapchain.GetSwapchainTexture();
using Framebuffer swapchainFramebuffer = device.CreateFramebuffer(swapchainTexture);

bool alive = true;
while (alive)
{
    while (window.PollEvent(out IWindowEvent winEvent))
    {
        switch (winEvent)
        {
            case QuitEvent:
                alive = false;
                break;
        }
    }
    
    commandList.Begin();

    RenderPassDescription renderPassDesc =
        new RenderPassDescription(swapchainFramebuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f));
    commandList.BeginRenderPass(renderPassDesc);
    commandList.EndRenderPass();
        
    commandList.End();
        
    device.ExecuteCommandList(commandList);
        
    swapchain.Present();
}