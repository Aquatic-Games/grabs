using System;

namespace grabs.Graphics;

public abstract class CommandList : IDisposable
{
    public abstract void Begin();

    public abstract void End();
    
    public abstract void BeginRenderPass(in RenderPassDescription description);

    public abstract void EndRenderPass();

    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, T data) where T : unmanaged
        => UpdateBuffer(buffer, offsetInBytes, (uint) sizeof(T), data);
    
    public void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, uint sizeInBytes, T data) where T : unmanaged
        => UpdateBuffer(buffer, offsetInBytes, sizeInBytes, new ReadOnlySpan<T>(ref data));

    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, in ReadOnlySpan<T> data) where T : unmanaged
        => UpdateBuffer(buffer, offsetInBytes, (uint) (data.Length * sizeof(T)), data);
    
    public unsafe void UpdateBuffer<T>(Buffer buffer, uint offsetInBytes, uint sizeInBytes, in ReadOnlySpan<T> data)
        where T : unmanaged
    {
        fixed (void* pData = data)
            UpdateBuffer(buffer, offsetInBytes, sizeInBytes, pData);
    }

    public abstract unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData);

    public abstract void GenerateMipmaps(Texture texture);

    public abstract void SetViewport(in Viewport viewport);

    public abstract void SetPipeline(Pipeline pipeline);

    public abstract void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset);

    public abstract void SetIndexBuffer(Buffer buffer, Format format);

    public abstract void SetDescriptorSet(/*uint binding, */DescriptorSet set);

    public abstract void DrawIndexed(uint numIndices);
    
    public abstract void Dispose();
}