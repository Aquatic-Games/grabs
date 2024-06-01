namespace grabs.Graphics.GL43;

// TODO: This can be done better, I don't like this at all.
public struct CommandListAction
{
    public CommandListActionType Type;
    
    public RenderPassDescription RenderPassDescription;
    
    public Pipeline Pipeline;
    public GL43DescriptorSet DescriptorSet;
    
    public uint Slot;
    public uint Stride;
    public uint Offset;
    public Format Format;
    
    public Buffer Buffer;
    public Viewport Viewport;
    public Texture Texture;

    public object MiscObject;

    public CommandListAction(CommandListActionType type)
    {
        Type = type;
    }
}