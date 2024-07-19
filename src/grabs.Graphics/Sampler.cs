using System;

namespace grabs.Graphics;

public abstract class Sampler : IDisposable
{
    public readonly SamplerDescription Description;

    protected Sampler(in SamplerDescription description)
    {
        Description = description;
    }
    
    public abstract void Dispose();
}