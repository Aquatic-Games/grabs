namespace grabs.Graphics;

public struct DescriptorSetDescription
{
    public Buffer Buffer;

    public DescriptorSetDescription(Buffer buffer = null)
    {
        Buffer = buffer;
    }
}