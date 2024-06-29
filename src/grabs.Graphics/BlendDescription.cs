using System.Numerics;

namespace grabs.Graphics;

public struct BlendDescription
{
    public bool IndependentBlending;

    public BlendAttachmentDescription[] Attachments;

    public Vector4 BlendConstants;

    public BlendDescription(BlendAttachmentDescription blendAttachment)
    {
        IndependentBlending = false;
        Attachments = [blendAttachment];
        BlendConstants = Vector4.Zero;
    }

    public BlendDescription(bool independentBlending, params BlendAttachmentDescription[] attachments)
    {
        IndependentBlending = independentBlending;
        Attachments = attachments;
        BlendConstants = Vector4.Zero;
    }

    public static BlendDescription Disabled => new BlendDescription(BlendAttachmentDescription.Disabled);

    public static BlendDescription NonPremultiplied =>
        new BlendDescription(BlendAttachmentDescription.NonPremultiplied);
}