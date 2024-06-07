using System;

namespace grabs.Audio;

public abstract class AudioDevice : IDisposable
{
    public readonly Context Context;

    protected AudioDevice(uint sampleRate)
    {
        Context = new Context(sampleRate);
    }

    protected unsafe void GetBuffer(Span<byte> buffer)
    {
        fixed (byte* pBuffer = buffer)
        {
            Span<float> floatBuffer = new Span<float>(pBuffer, buffer.Length / 4);
            Context.MixIntoBufferStereoF32(floatBuffer);
        }
    }

    public abstract void Dispose();
}