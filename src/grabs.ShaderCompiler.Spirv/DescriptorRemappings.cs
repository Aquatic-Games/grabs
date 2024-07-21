using System.Collections.Generic;

namespace grabs.ShaderCompiler.Spirv;

public class DescriptorRemappings
{
    public Dictionary<uint, Remapping> Sets;

    public DescriptorRemappings(Dictionary<uint, Remapping> sets)
    {
        Sets = sets;
    }

    public DescriptorRemappings()
    {
        Sets = new Dictionary<uint, Remapping>();
    }

    public bool TryGetRemappedSet(uint set, out Remapping remappedSet)
    {
        return Sets.TryGetValue(set, out remappedSet);
    }
}