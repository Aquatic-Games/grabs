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
            BeginRenderPass = new CommandListAction.BeginRenderPassAction()
            {
                RenderPassDescription = description
            }
        });
    }

    public override void EndRenderPass()
    {
        Actions.Add(new CommandListAction(CommandListActionType.EndRenderPass));
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        Actions.Add(new CommandListAction(CommandListActionType.SetPipeline)
        {
            SetPipeline = new CommandListAction.SetPipelineAction()
            {
                Pipeline = pipeline
            }
        });
    }

    public override void SetVertexBuffer(uint slot, Buffer buffer, uint stride, uint offset)
    {
        Actions.Add(new CommandListAction(CommandListActionType.SetVertexBuffer)
        {
            SetVertexBuffer = new CommandListAction.SetVertexBufferAction()
            {
                Slot = slot,
                Buffer = buffer,
                Stride = stride,
                Offset = offset
            }
        });
    }

    public override void SetIndexBuffer(Buffer buffer, Format format)
    {
        Actions.Add(new CommandListAction(CommandListActionType.SetIndexBuffer)
        {
            SetIndexBuffer = new CommandListAction.SetIndexBufferAction()
            {
                Buffer = buffer,
                Format = format
            }
        });
    }

    public override void DrawIndexed(uint numIndices)
    {
        Actions.Add(new CommandListAction(CommandListActionType.DrawIndexed)
        {
            Draw = new CommandListAction.DrawAction()
            {
                NumVerticesOrIndices = numIndices
            }
        });
    }

    public override void Dispose() { }
}