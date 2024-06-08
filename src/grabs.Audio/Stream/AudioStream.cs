using System;

namespace grabs.Audio.Stream;

public abstract class AudioStream : IDisposable
{
    public abstract AudioFormat Format { get; }
    
    public abstract byte[] GetPCM();
    
    public abstract void Dispose();
}