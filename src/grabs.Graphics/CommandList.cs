namespace grabs.Graphics;

/// <summary>
/// A command list contains a list of instructions used for rendering, which can then be executed by the GPU. 
/// </summary>
public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();

    public abstract void BeginRenderPass(in RenderPassInfo info);

    public abstract void EndRenderPass();

    public abstract void SetViewport(in Viewport viewport);

    public abstract void SetPipeline(Pipeline pipeline);

    public abstract void SetVertexBuffer(uint slot, Buffer vertexBuffer, uint offset = 0);

    public abstract void SetIndexBuffer(Buffer indexBuffer, Format format, uint offset = 0);

    public abstract void Draw(uint numVertices);

    public abstract void DrawIndexed(uint numIndices);
    
    public abstract void Dispose();
}