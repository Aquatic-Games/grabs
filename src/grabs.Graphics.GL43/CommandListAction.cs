using System.Runtime.InteropServices;

namespace grabs.Graphics.GL43;

public struct CommandListAction
{
    public CommandListActionType Type;

    public BeginRenderPassAction BeginRenderPass;

    public SetPipelineAction SetPipeline;

    public SetVertexBufferAction SetVertexBuffer;

    public SetIndexBufferAction SetIndexBuffer;

    public DrawAction Draw;

    public CommandListAction(CommandListActionType type)
    {
        Type = type;
    }

    public struct BeginRenderPassAction
    {
        public RenderPassDescription RenderPassDescription;
    }

    public struct SetPipelineAction
    {
        public Pipeline Pipeline;
    }

    public struct SetVertexBufferAction
    {
        public uint Slot;
        public Buffer Buffer;
        public uint Stride;
        public uint Offset;
    }

    public struct SetIndexBufferAction
    {
        public Buffer Buffer;
        public Format Format;
    }

    public struct DrawAction
    {
        public uint NumVerticesOrIndices;
    }
}