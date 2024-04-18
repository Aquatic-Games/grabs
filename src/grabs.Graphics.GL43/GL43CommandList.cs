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
        Actions.Add(new CommandListAction()
        {
            Type = CommandListActionType.EndRenderPass
        });
    }

    public override void SetPipeline(Pipeline pipeline)
    {
        throw new NotImplementedException();
    }

    public override void Dispose() { }
}