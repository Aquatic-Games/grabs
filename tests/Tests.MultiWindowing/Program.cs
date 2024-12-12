using System.Drawing;
using grabs.Graphics;
using grabs.Windowing;
using grabs.Windowing.Events;

const uint width = 800;
const uint height = 600;

using Window window1 = Window.Create(new WindowDescription(width, height, "Window 1", 100, 100));
using Window window2 = Window.Create(new WindowDescription(width, height, "Window 2", 1000, 400));

using Instance instance = Instance.Create(new InstanceDescription(true, Backend.Unknown), window1);

using Surface surface1 = window1.CreateSurface(instance);
using Surface surface2 = window2.CreateSurface(instance);

using Device device = instance.CreateDevice(surface1);

SwapchainDescription swapchainDesc =
    new SwapchainDescription(width, height, Format.B8G8R8A8_UNorm, 2, PresentMode.Fifo);
Console.WriteLine(swapchainDesc);

using Swapchain swapchain1 = device.CreateSwapchain(surface1, in swapchainDesc);
using Swapchain swapchain2 = device.CreateSwapchain(surface2, in swapchainDesc);

using CommandList cl = device.CreateCommandList();

while (Window.WindowCount > 0)
{
    while (Window.PollEvent(out WindowEvent winEvent))
    {
        switch (winEvent.Type)
        {
            case EventType.Quit:
                winEvent.Window.Dispose();
                break;
        }
    }
    
    cl.Begin();

    cl.BeginRenderPass(
        new RenderPassDescription(new ColorAttachmentDescription(swapchain1.GetNextTexture(), Color.RebeccaPurple)));
    cl.EndRenderPass();
    
    cl.BeginRenderPass(
        new RenderPassDescription(new ColorAttachmentDescription(swapchain2.GetNextTexture(), Color.CornflowerBlue)));
    cl.EndRenderPass();
    
    cl.End();
    device.ExecuteCommandList(cl);
    
    swapchain1.Present();
    swapchain2.Present();
}