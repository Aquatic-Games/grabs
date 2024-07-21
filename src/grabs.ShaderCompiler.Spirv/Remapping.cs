using System.Collections.Generic;

namespace grabs.ShaderCompiler.Spirv;

public class Remapping
{
    public readonly Dictionary<uint, uint> Bindings;

    public Remapping(Dictionary<uint, uint> bindings)
    {
        Bindings = bindings;
    }

    public Remapping()
    {
        Bindings = new Dictionary<uint, uint>();
    }

    public bool TryGetRemappedBinding(uint binding, out uint remappedBinding)
    {
        return Bindings.TryGetValue(binding, out remappedBinding);
    }
}