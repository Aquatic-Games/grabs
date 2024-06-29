namespace grabs.Graphics;

public struct BlendDescription
{
    public bool IndependentBlending;

    public BlendAttachmentDescription[] BlendAttachments;

    public BlendDescription(BlendAttachmentDescription blendAttachment)
    {
        IndependentBlending = false;
        BlendAttachments = [blendAttachment];
    }

    public BlendDescription(bool independentBlending, params BlendAttachmentDescription[] blendAttachments)
    {
        IndependentBlending = independentBlending;
        BlendAttachments = blendAttachments;
    }

    public static BlendDescription Disabled => new BlendDescription(BlendAttachmentDescription.Disabled);

    public static BlendDescription NonPremultiplied =>
        new BlendDescription(BlendAttachmentDescription.NonPremultiplied);
}