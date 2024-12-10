namespace grabs.Graphics;

/// <summary>
/// A list of rendering commands that can be sent to the GPU.
/// </summary>
public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();

    public abstract void BeginRenderPass(in RenderPassDescription description);

    public abstract void EndRenderPass();

    public abstract void SetViewport(in Viewport viewport);

    public abstract void SetPipeline(Pipeline pipeline);

    public abstract void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset);

    public abstract void SetIndexBuffer(Buffer buffer, Format format);

    public abstract void Draw(uint numVertices);

    public abstract void DrawIndexed(uint numIndices);
    
    public abstract void Dispose();
}