using System.Drawing;
using grabs.Graphics;
using grabs.Windowing;
using grabs.Windowing.Events;

const uint width = 1280;
const uint height = 720;

using Window window = Window.Create(new WindowDescription(width, height, "Test Window"));

using Instance instance = Instance.Create(new InstanceDescription(true, Backend.Unknown), window);
using Surface surface = window.CreateSurface(instance);

using Device device = instance.CreateDevice(surface);
using Swapchain swapchain = device.CreateSwapchain(surface,
    new SwapchainDescription(width, height, Format.B8G8R8A8_UNorm, 2, PresentMode.Fifo));
using CommandList cl = device.CreateCommandList();

bool alive = true;
while (alive)
{
    while (window.PollEvent(out IWindowEvent? winEvent))
    {
        switch (winEvent)
        {
            case QuitEvent:
                alive = false;
                break;
        }
    }
    
    cl.Begin();

    cl.BeginRenderPass(
        new RenderPassDescription(new ColorAttachmentDescription(swapchain.GetNextTexture(), Color.RebeccaPurple)));
    cl.EndRenderPass();
    
    cl.End();
    device.ExecuteCommandList(cl);
    
    swapchain.Present();
}



