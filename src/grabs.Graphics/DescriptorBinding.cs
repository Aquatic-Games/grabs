namespace grabs.Graphics;

public record struct DescriptorBinding
{
    public uint Binding;

    public DescriptorType Type;

    public ShaderStage Stages;

    public DescriptorBinding(uint binding, DescriptorType type, ShaderStage stages)
    {
        Binding = binding;
        Type = type;
        Stages = stages;
    }
}