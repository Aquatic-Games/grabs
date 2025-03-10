using grabs.Core;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.Vulkan;
using grabs.Windowing;
using grabs.Windowing.Events;

GrabsLog.LogMessage += (severity, source, message, _, _) => Console.WriteLine($"{severity} - {source}: {message}");  

WindowInfo windowInfo = new WindowInfo(new Size2D(1280, 720), "grabs.Windowing.Tests");

using Window window = new Window(in windowInfo);

Instance.RegisterBackend<D3D11Backend>();
Instance.RegisterBackend<VulkanBackend>();
using Instance instance = Instance.Create(new InstanceInfo("grabs.Windowing.Tests", debug: true));

using Surface surface = window.CreateSurface(instance);
using Device device = instance.CreateDevice(surface);

Format format = surface.GetOptimalSwapchainFormat(device.Adapter);

using Swapchain swapchain =
    device.CreateSwapchain(new SwapchainInfo(surface, windowInfo.Size, format, PresentMode.Fifo, 2));

using CommandList cl = device.CreateCommandList();

bool alive = true;
while (alive)
{
    while (window.PollEvent(out Event winEvent))
    {
        switch (winEvent.Type)
        {
            case EventType.Close:
                alive = false;
                break;
        }
    }

    Texture texture = swapchain.GetNextTexture();
    
    cl.Begin();
    cl.BeginRenderPass(new RenderPassInfo(new ColorAttachmentInfo(texture, new ColorF(1.0f, 0.5f, 0.25f))));
    cl.EndRenderPass();
    cl.End();
    
    device.ExecuteCommandList(cl);
    swapchain.Present();
}