using System;

namespace grabs.Audio.Devices;

public abstract class AudioDevice : IDisposable
{
    public readonly Context Context;

    protected AudioDevice(uint sampleRate)
    {
        Context = new Context(sampleRate);
    }

    protected void GetBuffer(Span<byte> buffer)
    {
        
    }

    public abstract void Dispose();
}