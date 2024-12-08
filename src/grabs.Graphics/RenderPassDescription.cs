namespace grabs.Graphics;

public ref struct RenderPassDescription
{
    public ReadOnlySpan<ColorAttachmentDescription> ColorAttachments;

    public RenderPassDescription(ref readonly ReadOnlySpan<ColorAttachmentDescription> colorAttachments)
    {
        ColorAttachments = colorAttachments;
    }

    public RenderPassDescription(ref readonly ColorAttachmentDescription colorAttachment)
    {
        ColorAttachments = new ReadOnlySpan<ColorAttachmentDescription>(in colorAttachment);
    }
}