using System;

namespace grabs.Graphics;

public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();
    
    public abstract void BeginRenderPass(in RenderPassDescription description);

    public abstract void EndRenderPass();
    
    public abstract void Dispose();
}