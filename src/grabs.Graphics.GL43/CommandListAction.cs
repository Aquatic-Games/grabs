using System.Runtime.InteropServices;

namespace grabs.Graphics.GL43;

[StructLayout(LayoutKind.Explicit)]
public struct CommandListAction
{
    [FieldOffset(0)]
    public CommandListActionType Type;

    [FieldOffset(4)] public BeginRenderPassAction BeginRenderPass;

    public struct BeginRenderPassAction
    {
        public RenderPassDescription RenderPassDescription;
    }
}