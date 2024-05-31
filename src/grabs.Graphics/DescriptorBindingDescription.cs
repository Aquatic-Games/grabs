namespace grabs.Graphics;

public struct DescriptorBindingDescription
{
    public uint Binding;
    
    public DescriptorType Type;

    public ShaderStage Stages;

    public DescriptorBindingDescription(uint binding, DescriptorType type, ShaderStage stages)
    {
        Binding = binding;
        Type = type;
        Stages = stages;
    }
}