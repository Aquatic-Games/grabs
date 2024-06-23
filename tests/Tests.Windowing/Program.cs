using System;
using System.Drawing;
using System.Numerics;
using grabs.Graphics;
using grabs.Windowing;
using grabs.Windowing.Events;

WindowInfo info = new WindowInfo("Test Window", new Size(1280, 720));

using Window window = new Window(info);
using Instance instance = window.CreateInstance();
using Surface surface = window.CreateSurface();

using Device device = instance.CreateDevice(surface);

using Swapchain swapchain =
    device.CreateSwapchain(new SwapchainDescription(1280, 720, presentMode: PresentMode.VerticalSync));
using Texture swapchainTexture = swapchain.GetSwapchainTexture();
using Framebuffer swapchainBuffer = device.CreateFramebuffer(new ReadOnlySpan<Texture>(in swapchainTexture));

using CommandList cl = device.CreateCommandList();

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
    
    cl.Begin();
    
    cl.BeginRenderPass(new RenderPassDescription(swapchainBuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
    cl.EndRenderPass();
    
    cl.End();
    
    device.ExecuteCommandList(cl);

    swapchain.Present();
}