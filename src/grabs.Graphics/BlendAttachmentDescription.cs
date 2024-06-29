namespace grabs.Graphics;

public struct BlendAttachmentDescription
{
    public bool Enabled;

    public BlendFactor Source;

    public BlendFactor Destination;

    public BlendOperation BlendOperation;

    public BlendFactor SourceAlpha;

    public BlendFactor DestinationAlpha;

    public BlendOperation AlphaBlendOperation;

    public ColorWriteMask ColorWriteMask;

    public BlendAttachmentDescription(bool enabled, BlendFactor source, BlendFactor destination) 
        : this(enabled, source, destination, BlendOperation.Add, source, destination, BlendOperation.Add, ColorWriteMask.All) { }

    public BlendAttachmentDescription(bool enabled, BlendFactor source, BlendFactor destination,
        BlendOperation blendOperation, BlendFactor sourceAlpha, BlendFactor destinationAlpha,
        BlendOperation alphaBlendOperation, ColorWriteMask colorWriteMask)
    {
        Enabled = enabled;
        Source = source;
        Destination = destination;
        BlendOperation = blendOperation;
        SourceAlpha = sourceAlpha;
        DestinationAlpha = destinationAlpha;
        AlphaBlendOperation = alphaBlendOperation;
        ColorWriteMask = colorWriteMask;
    }

    public static BlendAttachmentDescription Disabled =>
        new BlendAttachmentDescription(false, BlendFactor.One, BlendFactor.Zero);

    public static BlendAttachmentDescription NonPremultiplied =>
        new BlendAttachmentDescription(true, BlendFactor.SrcAlpha, BlendFactor.OneMinusSrcAlpha);
}