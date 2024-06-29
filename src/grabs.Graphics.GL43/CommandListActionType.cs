namespace grabs.Graphics.GL43;

public enum CommandListActionType
{
    BeginRenderPass,
    EndRenderPass,
    UpdateBuffer,
    GenerateMipmaps,
    SetViewport,
    SetPipeline,
    SetVertexBuffer,
    SetIndexBuffer,
    SetDescriptor,
    Draw,
    DrawIndexed,
    DrawIndexedBaseVertex
}