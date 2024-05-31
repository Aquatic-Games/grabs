using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace grabs.Graphics.GL43;

public class GL43CommandList : CommandList
{
    public List<CommandListAction> Actions;

    public GL43CommandList()
    {
        Actions = new List<CommandListAction>();
    }
    
    public override void Begin()
    {
        Actions.Clear();
    }

    public override void End() { }

    public override void BeginRenderPass(in RenderPassDescription description)
    {
        Actions.Add(new CommandListAction()
        {
            Type = CommandListActionType.BeginRenderPass,
            RenderPassDescription = description
        });
    }

    public override void EndRenderPass()
    {
        Actions.Add(new CommandListAction(CommandListActionType.EndRenderPass));
    }

    public override unsafe void UpdateBuffer(Buffer buffer, uint offsetInBytes, uint sizeInBytes, void* pData)
    {
        // I REALLY hate this but it works. The OpenGL backend is not the most performant anyway.
        // TODO: Is there a better solution?
        byte[] dataArray = new byte[sizeInBytes];
        fixed (void* pDataArray = dataArray)
            Unsafe.CopyBlock(pDataArray, pData, sizeInBytes);
        
        Actions.Add(new CommandListAction(CommandListActionType.UpdateBuffer)
        {
            Buffer = buffer,
            Stride = sizeInBytes,
            Offset = offsetInBytes,
            MiscObject = dataArray
        });
    }

    public override void GenerateMipmaps(Texture texture)
    {
        Actions.Add(new CommandListAction(CommandListActionType.GenerateMipmaps)
        {
            Texture = texture
        });
    }

    public override void SetViewport(in Viewport viewport)
    {
        Actions.Add(new CommandListAction(CommandListActionType.SetViewport)
        {
            Viewport = viewport
        });
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        Actions.Add(new CommandListAction(CommandListActionType.SetPipeline)
        {
            Pipeline = pipeline
        });
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset)
    {
        Actions.Add(new CommandListAction(CommandListActionType.SetVertexBuffer)
        {
            Slot = slot,
            Buffer = buffer,
            Stride = stride,
            Offset = offset
        });
    }

    public override void SetIndexBuffer(Buffer buffer, Format format)
    {
        Actions.Add(new CommandListAction(CommandListActionType.SetIndexBuffer)
        {
            Buffer = buffer,
            Format = format
        });
    }

    public override void DrawIndexed(uint numIndices)
    {
        Actions.Add(new CommandListAction(CommandListActionType.DrawIndexed)
        {
            Slot = numIndices
        });
    }

    public override void Dispose() { }
}