namespace grabs.Graphics.GL43;

// TODO: This can be done better, I don't like this at all.
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

    public Viewport Viewport;

    public object MiscObject;

    public CommandListAction(CommandListActionType type)
    {
        Type = type;
    }
}