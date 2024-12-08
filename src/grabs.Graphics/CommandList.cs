namespace grabs.Graphics;

/// <summary>
/// A list of rendering commands that can be sent to the GPU.
/// </summary>
public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();

    public abstract void BeginRenderPass(ref readonly RenderPassDescription description);

    public abstract void EndRenderPass();
    
    public abstract void Dispose();
}