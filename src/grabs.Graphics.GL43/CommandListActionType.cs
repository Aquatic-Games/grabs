namespace grabs.Graphics.GL43;

public enum CommandListActionType
{
    BeginRenderPass,
    EndRenderPass,
    UpdateBuffer,
    GenerateMipmaps,
    SetViewport,
    SetScissor,
    SetPipeline,
    SetVertexBuffer,
    SetIndexBuffer,
    SetDescriptor,
    Draw,
    DrawIndexed,
    DrawIndexedBaseVertex
}