using System.Runtime.InteropServices;

namespace grabs.Graphics.GL43;

public struct CommandListAction
{
    public CommandListActionType Type;

    public BeginRenderPassAction BeginRenderPass;

    public struct BeginRenderPassAction
    {
        public RenderPassDescription RenderPassDescription;
    }
}