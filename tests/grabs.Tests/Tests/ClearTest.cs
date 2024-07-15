using System.Numerics;
using grabs.Graphics;

namespace grabs.Tests.Tests;

public class ClearTest : TestBase
{
    public ClearTest() : base("Clearing Test") { }

    protected override void Draw()
    {
        base.Draw();
        
        CommandList.Begin();
        
        CommandList.BeginRenderPass(new RenderPassDescription(Framebuffer, new Vector4(1.0f, 0.5f, 0.25f, 1.0f)));
        CommandList.EndRenderPass();
        
        CommandList.End();

        Device.ExecuteCommandList(CommandList);
    }
}