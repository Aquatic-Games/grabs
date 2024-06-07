using System;

namespace grabs.Audio.Internal;

internal struct Buffer
{
    public byte[] Data;
    public AudioFormat Format;

    public Buffer(byte[] data, AudioFormat format)
    {
        Data = data;
        Format = format;
    }
}