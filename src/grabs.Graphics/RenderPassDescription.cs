namespace grabs.Graphics;

public ref struct RenderPassDescription
{
    public ReadOnlySpan<ColorAttachmentDescription> ColorAttachments;

    public RenderPassDescription(in ReadOnlySpan<ColorAttachmentDescription> colorAttachments)
    {
        ColorAttachments = colorAttachments;
    }

    public RenderPassDescription(in ColorAttachmentDescription colorAttachment)
    {
        ColorAttachments = new ReadOnlySpan<ColorAttachmentDescription>(in colorAttachment);
    }
}