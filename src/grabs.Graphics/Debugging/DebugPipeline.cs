namespace grabs.Graphics.Debugging;

internal sealed class DebugPipeline : Pipeline
{
    public override bool IsDisposed
    {
        get => Pipeline.IsDisposed;
        protected set => throw new NotImplementedException();
    }

    public readonly Pipeline Pipeline;

    public readonly Format[] ColorAttachmentFormats;

    public DebugPipeline(Device device, ref readonly GraphicsPipelineInfo info)
    {
        ColorAttachmentFormats = new Format[info.ColorAttachments.Length];
        for (int i = 0; i < info.ColorAttachments.Length; i++)
            ColorAttachmentFormats[i] = info.ColorAttachments[i].Format;

        Pipeline = device.CreateGraphicsPipeline(in info);
    }
    
    public override void Dispose()
    {
        Pipeline.Dispose();
    }
}