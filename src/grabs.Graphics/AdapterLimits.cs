namespace grabs.Graphics;

/// <summary>
/// Represents various limits for an <see cref="Adapter"/>.
/// </summary>
public readonly record struct AdapterLimits
{
    public readonly uint MaxColorAttachments;
    
    public readonly uint MaxPushConstantSize;

    public readonly uint MaxAnisotropyLevels;
    
    public AdapterLimits(uint maxColorAttachments, uint maxPushConstantSize, uint maxAnisotropyLevels)
    {
        MaxColorAttachments = maxColorAttachments;
        MaxPushConstantSize = maxPushConstantSize;
        MaxAnisotropyLevels = maxAnisotropyLevels;
    }
}