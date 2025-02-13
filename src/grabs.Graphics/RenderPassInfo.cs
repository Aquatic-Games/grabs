namespace grabs.Graphics;

public ref struct RenderPassInfo
{
    public ReadOnlySpan<ColorAttachmentInfo> ColorAttachments;

    public RenderPassInfo(in ReadOnlySpan<ColorAttachmentInfo> colorAttachments)
    {
        ColorAttachments = colorAttachments;
    }

    public RenderPassInfo(params ColorAttachmentInfo[] colorAttachments)
    {
        ColorAttachments = colorAttachments;
    }

    public RenderPassInfo(in ColorAttachmentInfo colorAttachment)
    {
        ColorAttachments = new ReadOnlySpan<ColorAttachmentInfo>(in colorAttachment);
    }
}