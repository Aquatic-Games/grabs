using grabs.Core;
using grabs.Graphics;
using grabs.Windowing;
using grabs.Windowing.Events;

/*
 * Tutorial 1.1 - Basic Window
 * Shows you how to create a window and set up the basic objects required to clear the screen.
 */

// Create our window. We describe various parameters of the window using the WindowInfo struct. 
WindowInfo windowInfo = new WindowInfo()
{
    Size = new Size2D(1280, 720),
    Title = "Tutorial.1.1.BasicWindow"
};
using Window window = new Window(in windowInfo);

// An instance is the base on which we can enumerate adapters, create devices, etc.
// There are various parameters we can pass into the InstanceInfo struct. Here, we only pass in the app name.
// You can also enable debugging and choose a preferred Backend, if any.
InstanceInfo instanceInfo = new InstanceInfo("Tutorial.1.1.BasicWindow");
using Instance instance = Instance.Create(in instanceInfo);

// In order to render to a window, we must create a surface to render to.
using Surface surface = window.CreateSurface(instance);

// Create our logical device that we will use for rendering.
using Device device = instance.CreateDevice(surface);

// A swapchain contains a series of textures (images) that are presented to the surface.
SwapchainInfo swapchainInfo = new()
{
    Size = window.Size,
    // While you can pass in any format you'd like (as long as it's supported by the surface), GRABS has a utility
    // method that will pick the optimal format for you.
    Format = surface.GetOptimalSwapchainFormat(device.Adapter),
    // The present mode determines how the presentation will occur - in this case we are using FIFO, which is equivalent
    // to having V-Sync enabled.
    PresentMode = PresentMode.Fifo
};
using Swapchain swapchain = device.CreateSwapchain(surface, in swapchainInfo);

// A command list contains a series of render commands that can be sent to the device.
using CommandList commandList = device.CreateCommandList();

bool alive = true;
while (alive)
{
    // Run our event loop.
    while (window.PollEvent(out Event winEvent))
    {
        switch (winEvent.Type)
        {
            case EventType.Close:
                alive = false;
                break;
        }
    }

    // Get the next texture from the swapchain that can be rendered to.
    Texture texture = swapchain.GetNextTexture();
    
    // Begin our command list so it can start accepting commands.
    commandList.Begin();

    // Any time we want to draw, we must do it inside a render pass.
    RenderPassInfo passInfo = new()
    {
        ColorAttachments =
        [
            // Every render pass must contain at least 1 color attachment.
            // We'll go into detail about this later, but for now we can just provide our swapchain texture.
            // We also tell it that we want to clear the texture, and what color it should be cleared to.
            new ColorAttachmentInfo()
            {
                Texture = texture,
                ClearColor = new ColorF(1.0f, 0.5f, 0.25f),
                LoadOp = LoadOp.Clear
            }
        ]
    };
    // Begin and immediately end our render pass, since we are not providing any other commands.
    commandList.BeginRenderPass(in passInfo);
    commandList.EndRenderPass();
    
    // End our command list so we can prepare to execute it.
    commandList.End();
    
    // Submit the command list to the device and execute it.
    device.ExecuteCommandList(commandList);
    
    // Finally, present the swapchain to the surface.
    swapchain.Present();
}