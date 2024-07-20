namespace grabs.Graphics;

public struct DescriptorSetDescription
{
    public Buffer Buffer;

    public Texture Texture;

    public Sampler Sampler;

    public DescriptorSetDescription(Buffer buffer = null, Texture texture = null, Sampler sampler = null)
    {
        Buffer = buffer;
        Texture = texture;
        Sampler = sampler;
    }
}