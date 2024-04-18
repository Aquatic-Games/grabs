namespace grabs.Graphics.GL43;

public struct CommandListAction
{
    public CommandListActionType Type;
    
    public RenderPassDescription RenderPassDescription;
    
    public Pipeline Pipeline;
    
    public Buffer Buffer;
    public uint Slot;
    public uint Stride;
    public uint Offset;
    public Format Format;

    public object MiscObject;

    public CommandListAction(CommandListActionType type)
    {
        Type = type;
    }
}