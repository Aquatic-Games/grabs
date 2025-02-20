using grabs.Core;
using grabs.Graphics;
using grabs.Windowing;
using grabs.Windowing.Events;

WindowInfo windowInfo = new WindowInfo(new Size2D(1280, 720), "Tutorial.1.1.BasicWindow");
using Window window = new Window(in windowInfo);

InstanceInfo instanceInfo = new InstanceInfo("Tutorial.1.1.BasicWindow");
using Instance instance = Instance.Create(in instanceInfo);

using Surface surface = window.CreateSurface(instance);

using Device device = instance.CreateDevice(surface);

SwapchainInfo swapchainInfo = new()
{
    Size = window.Size,
    Format = surface.GetOptimalSwapchainFormat(device.Adapter),
    PresentMode = PresentMode.Fifo
};
using Swapchain swapchain = device.CreateSwapchain(surface, in swapchainInfo);

using CommandList commandList = device.CreateCommandList();

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
    
    commandList.Begin();

    RenderPassInfo passInfo = new()
    {
        ColorAttachments =
        [
            new ColorAttachmentInfo()
            {
                Texture = texture,
                ClearColor = new ColorF(1.0f, 0.5f, 0.25f),
                LoadOp = LoadOp.Clear
            }
        ]
    };
    
    commandList.BeginRenderPass(in passInfo);
    commandList.EndRenderPass();
    
    commandList.End();
    
    device.ExecuteCommandList(commandList);
    swapchain.Present();
}