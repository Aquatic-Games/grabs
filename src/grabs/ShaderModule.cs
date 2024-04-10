using System;

namespace grabs;

public abstract class ShaderModule : IDisposable
{
    public readonly ShaderStage Stage;

    protected ShaderModule(ShaderStage stage)
    {
        Stage = stage;
    }
    
    public abstract void Dispose();
}