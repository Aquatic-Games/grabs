using System;

namespace grabs.Graphics;

public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();
    
    public abstract void BeginRenderPass(in RenderPassDescription description);

    public abstract void EndRenderPass();

    public abstract void SetPipeline(Pipeline pipeline);

    public abstract void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset);

    public abstract void SetIndexBuffer(Buffer buffer, Format format);

    public abstract void SetConstantBuffer(uint slot, Buffer buffer);

    public abstract void DrawIndexed(uint numIndices);
    
    public abstract void Dispose();
}