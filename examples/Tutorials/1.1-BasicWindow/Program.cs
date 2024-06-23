using System.Drawing;
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
WindowInfo windowInfo = new WindowInfo("Tutorial 1.1 - Basic Window", new Size((int) width, (int) height));
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

// Create a swapchain that can be rendered to.
// This defines the set of textures, and the framebuffer, that make up the final result.
// Typically, a swapchain contains at least 2 buffers (in this case, textures), however you can define the number you
// want in the Swapchain Description, although the default value is 2.
// GRABS only exposes the first texture in the chain, and automatically switches texture depending on which buffer
// you're rendering to.
SwapchainDescription swapchainDesc = new SwapchainDescription(width, height, presentMode: PresentMode.VerticalSync);
using Swapchain swapchain = device.CreateSwapchain(swapchainDesc);
using Texture swapchainTexture = swapchain.GetSwapchainTexture();
using Framebuffer swapchainFramebuffer = device.CreateFramebuffer(swapchainTexture);

bool alive = true;
while (alive)
{
    // Our main event loop. Keep polling all events until there are none left.
    // In this case, we are just listening to the quit event.
    while (window.PollEvent(out IWindowEvent winEvent))
    {
        switch (winEvent)
        {
            case QuitEvent:
                alive = false;
                break;
        }
    }
    
    // Begin our command list. This prepares it to start accepting commands.
    commandList.Begin();

    // Begin a render pass. Every render command must be inside a render pass.
    // Each render pass starts by clearing the given framebuffer (unless we tell it to preserve the contents),
    // and since that is all we are doing in this example, we just immediately end the render pass.
    RenderPassDescription renderPassDesc =
        new RenderPassDescription(swapchainFramebuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f));
    commandList.BeginRenderPass(renderPassDesc);
    commandList.EndRenderPass();
    
    // End and finalize the command list for this frame.
    commandList.End();
    
    // Tell the device to execute our command list. While in this example, we overwrite our command list each frame,
    // you can execute the command list as many times as you want.
    device.ExecuteCommandList(commandList);
    
    // Finally, present to the window's surface.
    // This is also known as "swapping buffers".
    swapchain.Present();
}