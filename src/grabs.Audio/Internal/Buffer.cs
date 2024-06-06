using System;
using System.Runtime.InteropServices;

namespace grabs.Audio.Internal;

internal unsafe struct Buffer : IDisposable
{
    public byte* Data;
    public AudioFormat Format;

    public Buffer(byte* data, AudioFormat format)
    {
        Data = data;
        Format = format;
    }

    public void Dispose()
    {
        NativeMemory.Free(Data);
    }
}