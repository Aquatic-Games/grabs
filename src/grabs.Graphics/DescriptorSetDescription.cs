namespace grabs.Graphics;

public struct DescriptorSetDescription
{
    public Buffer Buffer;

    public Texture Texture;

    public DescriptorSetDescription(Buffer buffer = null, Texture texture = null)
    {
        Buffer = buffer;
        Texture = texture;
    }
}