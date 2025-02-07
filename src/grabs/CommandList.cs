namespace grabs;

public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();

    public abstract void BeginRenderPass(in RenderPassInfo info);

    public abstract void EndRenderPass();
    
    public abstract void Dispose();
}